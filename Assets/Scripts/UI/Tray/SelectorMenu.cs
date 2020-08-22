﻿using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.State;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/**
 * Populates and manages the “Shelf” menu in Tray mode.
 */
public class SelectorMenu : MonoBehaviour
{
    private bool byLab;
    private RegionData region = null;
    private string organ = "";
    private string courseId = "";
    private int labId = 0;

    [Header("Services")] public SpecimenStore store;
    public TrayPage trayPage;
    public StateController stateController;

    [Header("Prefabs")] public SelectorButton selectorPrefab;
    public SelectorButton lightSelectorPrefab;
    public CourseOption coursePrefab;
    public LabOption labPrefab;
    public SelectorButton noSpecimensPrefab;
    public SelectorButton specimenSelectorPrefab;
    public Button seeAllButtonPrefab;

    [Header("Internal Structures")] public Transform listTransform;
    public TextMeshProUGUI backBttnTitle;
    public Button backButton;
    public TextMeshProUGUI selectionTitle;
    public GameObject labAtlasToggle;
    public Button atlasButton;
    public Button labButton;
    public TextMeshProUGUI atlasLabel;
    public TextMeshProUGUI labLabel;
    public GameObject loadingIndicator;
    public Animator anim;

    private ListMode currentMode;
    private Dictionary<string, SelectorButton> idToButton = new Dictionary<string, SelectorButton>();

    private const string COURSES = "COURSES";
    private const string LABS = "LABS";
    private const string SHELF = "SHELF";
    private const string SPECIMEN_LIST = "SPECIMEN LIST";

    public enum ListMode
    {
        REGION,
        REGION_EXPANDED,
        SPECIMEN,
        LAB,
        LAB_COURSES,
        LAB_SPECIMENS
    }

    private List<RegionData> _loadedRegions;
    private List<string> _loadedOrgans;
    private List<SpecimenData> _loadedSpecimens;
    private bool _loading = true;
    private List<LabData> _loadedLabs;
    private List<CourseData> _loadedCourses;

    void Start()
    {
        if (store == null) store = FindObjectOfType<SpecimenStore>();


        selectionTitle.text = "LOADING SPECIMENS...";
        labButton.onClick.AddListener(ToggleToLabs);
        atlasButton.onClick.AddListener(ToggleToAtlas);

        ToggleToLabs();
    }

    void Update()
    {
        if (_loading && !store.Loading())
        {
            _loading = false;
            Populate();
        }
    }

    /**
     * Determines current population of the ui list and calls layout to output data.
     */
    public void Populate()
    {
        selectionTitle.text = "";
        ListMode mode = ListMode.LAB_COURSES;

        // Sets current list mode based on set fields.
        // Then prepares requested data:
        if (store.Loading())
        {
            // do nothing, as the data used to populate the UI hasn't loaded yet
        } else if (byLab)
        {
            if (courseId.Length < 1)
            {
                backBttnTitle.text = SHELF;
                _loadedCourses = store.labCourses.Values.ToList();
                _loadedCourses.Sort((c1, c2) => c1.courseId.CompareTo(c2.courseId)); // sort courses alphebetically
            }
            else if (labId > 0)
            {
                mode = ListMode.LAB_SPECIMENS;
                backBttnTitle.text = LABS;
                Tuple<string, List<SpecimenData>> labData = store.GetLabData(courseId, labId);
                selectionTitle.text = labData.Item1;
                _loadedSpecimens = labData.Item2;
            }
            else
            {
                mode = ListMode.LAB;
                backBttnTitle.text = COURSES;
                selectionTitle.text = courseId;
                _loadedLabs = store.GetLabDataForCourse(courseId);
            }
        }
        else if (region == null)
        {
            mode = ListMode.REGION;
            backBttnTitle.text = SHELF;
            _loadedRegions = store.regions.OrderBy(r => r.order).ToList();
        }
        else if (string.IsNullOrEmpty(organ))
        {
            mode = ListMode.REGION_EXPANDED;
            backBttnTitle.text = SHELF;
            _loadedRegions = store.regions.OrderBy(r => r.order).ToList();
            _loadedOrgans = store.GetOrgansByRegion(region.name);
        }
        else
        {
            mode = ListMode.SPECIMEN;
            backBttnTitle.text = SPECIMEN_LIST;
            selectionTitle.text = organ;
            _loadedOrgans = store.specimensByRegionByOrgan[region.name].Keys.ToList();
            _loadedSpecimens = store.specimensByRegionByOrgan[region.name][organ];
        }

        Layout(mode);
    }

    public void SelectCompare()
    {
        region = null;
        organ = "";
        Populate();
    }

    public void EndCompare()
    {
        Populate();
    }

    public void CourseSelected(string courseId)
    {
        this.courseId = courseId;
        Populate();
    }

    public void LabSelected(int labId)
    {
        this.labId = labId;
        Populate();
    }

    /**
     * Lays out UI based on requested mode.
     * Called only through Populate() to ensure that correct data is available.
     */
    private void Layout(ListMode mode)
    {
        loadingIndicator.gameObject.SetActive(store.Loading());
        selectionTitle.gameObject.SetActive(selectionTitle.text.Length > 0);
        labAtlasToggle.SetActive(
            !store.Loading() &&
            mode != ListMode.SPECIMEN &&
            mode != ListMode.LAB_SPECIMENS
        );

        // Clears data.
        Clear();
        idToButton = new Dictionary<string, SelectorButton>();
        currentMode = mode;

        if (store.Loading())
        {
            // all we needed to do was show the loading indicator, so we are done laying out the UI
            return;
        }

        if (mode == ListMode.LAB_COURSES)
        {
            _loadedCourses.ForEach((course) =>
            {
                CourseOption courseOption = Instantiate(coursePrefab, listTransform);
                courseOption.Populate(course.courseId, this);
            });

            backButton.onClick.AddListener(trayPage.ToggleShelfMenu);
            return;
        }

        if (mode == ListMode.LAB)
        {
            for (int i = 0; i < _loadedLabs.Count; i++)
            {
                LabOption lo = Instantiate(labPrefab, listTransform);
                lo.Populate(_loadedLabs[i], this);
            }

            backButton.onClick.AddListener(ClearSelectionData);
            return;
        }

        if (mode == ListMode.SPECIMEN || mode == ListMode.LAB_SPECIMENS)
        {
            // Forgive me for the spaghetti below
            // Loops through all loaded specimens of organ type and produces a clickable button for each.
            for (int i = 0; i < _loadedSpecimens.Count; i++)
            {
                string id = _loadedSpecimens[i].id;
                SelectorButton btn = Instantiate(specimenSelectorPrefab, listTransform);
                btn.Populate(_loadedSpecimens[i].name, i, null);
                idToButton.Add(id, btn);
            }

            if (stateController.CurrentSpecimenData != null && trayPage.selectingCompareSpecimen)
            {
                Button btn = Instantiate(seeAllButtonPrefab, listTransform);
                btn.onClick.AddListener(ClearOrganAndLabData);
            }

            // Activates the back button, which takes user back to Region/Organ list
            backButton.onClick.AddListener(ClearOrganAndLabData);
            UpdateSelected();
        }
        else
        {
            // Loops through loaded regions, producing a clickable button for each...
            for (int i = 0; i < _loadedRegions.Count; i++)
            {
                SelectorButton btn = Instantiate(selectorPrefab, listTransform);
                btn.Populate(_loadedRegions[i].name, i, _loadedRegions[i].icon);
                idToButton.Add(_loadedRegions[i].name, btn);

                // If a region is the currently selected, output the organs found as buttons below.
                if (_loadedRegions[i] == region)
                {
                    btn.ShowBackground(true);
                    btn.children.gameObject.SetActive(true);

                    if (_loadedOrgans.Count == 0)
                    {
                        SelectorButton sbtn = Instantiate(noSpecimensPrefab, btn.children);
                        sbtn.button.onClick.AddListener(UnselectRegion);
                    }
                    else
                    {
                        for (int j = 0; j < _loadedOrgans.Count; j++)
                        {
                            SelectorButton sbtn = Instantiate(lightSelectorPrefab, btn.children);
                            sbtn.Populate(_loadedOrgans[j], j, null);


                            // Bind a click listener that loads the specimen selection view
                            sbtn.button.onClick.AddListener(() => { SelectOrgan(_loadedOrgans[sbtn.indexValue]); });
                            idToButton.Add(_loadedOrgans[sbtn.indexValue], sbtn);
                        }
                    }

                    // Bind a click listener that closes the region accordion
                    btn.button.onClick.AddListener(UnselectRegion);
                }
                else
                {
                    btn.children.gameObject.SetActive(false);
                    // Bind a click listener that closes the current region accordion and opens a new one
                    btn.button.onClick.AddListener(() => { SelectRegion(_loadedRegions[btn.indexValue]); });
                }
            }

            // Bind a click listener that toggles the shelf menu
            backButton.onClick.AddListener(trayPage.ToggleShelfMenu);
        }
    }

    private void Clear()
    {
        backButton.onClick.RemoveAllListeners();

        // Clears all menu options
        foreach (Transform child in listTransform)
        {
            Destroy(child.gameObject);
        }
    }

    private void SelectRegion(RegionData region)
    {
        this.region = region;
        Populate();
    }

    private void SelectOrgan(string organ)
    {
        this.organ = organ;
        Populate();
    }

    private void SelectSpecimen(string specimenId)
    {
        SetSpecimenButtonToSelected(specimenId);
        trayPage.SpecimenSelected(store.specimens[specimenId]);
    }

    private void UnselectRegion()
    {
        region = null;
        Populate();
    }

    private void ClearOrganAndLabData()
    {
        organ = "";
        labId = 0;
        Populate();
    }

    private void ClearSelectionData()
    {
        courseId = "";
        region = null;
        ClearOrganAndLabData();
    }

    private void ToggleToAtlas()
    {
        labButton.interactable = true;
        atlasButton.interactable = false;
        atlasLabel.color = Color.white;
        labLabel.color = Color.black;
        byLab = false;
        ClearSelectionData();
    }

    private void ToggleToLabs()
    {
        labButton.interactable = false;
        atlasButton.interactable = true;
        labLabel.color = Color.white;
        atlasLabel.color = Color.black;
        byLab = true;
        ClearSelectionData();
    }

    private void SetSpecimenButtonToSelected(string specId)
    {
        if (!idToButton.ContainsKey(specId)) return; //Current specimen not on screen
        SelectorButton btn = idToButton[specId];
        btn.ShowBackground(true);
        btn.icon.gameObject.SetActive(true);
        btn.button.onClick.RemoveAllListeners();
        btn.button.onClick.AddListener(() =>
        {
            trayPage.RemoveEitherActiveSpecimen(specId);
            UpdateSelected();
        });
    }

    private void SetSpecimenButtonToDeselected(string specId)
    {
        if (!idToButton.ContainsKey(specId)) return; //Current specimen not on screen
        SelectorButton btn = idToButton[specId];
        btn.ShowBackground(false);
        btn.icon.gameObject.SetActive(false);
        btn.button.onClick.RemoveAllListeners();
        btn.button.onClick.AddListener(() => {
            SelectSpecimen(_loadedSpecimens[btn.indexValue].id);
            btn.SetLoadingUntil(() => store.specimens[_loadedSpecimens[btn.indexValue].id].dataLoaded);
            UpdateSelected();
        });
    }

    public void SetOrganRegion(string organId, RegionData regionData)
    {
        organ = organId;
        region = regionData;
        Populate();
    }

    // Scans all active selector buttons and sets them to active if they are a selected specimen
    public void UpdateSelected()
    {
        if (currentMode != ListMode.SPECIMEN && currentMode != ListMode.LAB_SPECIMENS) return;
        string primaryId = null;
        string compareId = null;

        if (stateController.currentSpecimenId != null) {
            primaryId = stateController.currentSpecimenId;
        }
        if (stateController.CompareSpecimenData != null) {
            compareId = stateController.CompareSpecimenData.id;
        }

        foreach (string key in idToButton.Keys)
        {
            if (key == primaryId || key == compareId)
            {
                SetSpecimenButtonToSelected(key);
            }
            else
            {
                SetSpecimenButtonToDeselected(key);
            }
        }
    }

    // Called by EventTrigger on object
    public void HoverShelfToggle()
    {
        anim.SetBool("PeekMenu", true);
    }

    public void UnhoverShelfToggle()
    {
        anim.SetBool("PeekMenu", false);
    }
}