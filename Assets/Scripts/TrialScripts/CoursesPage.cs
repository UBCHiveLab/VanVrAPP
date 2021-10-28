using Assets.Scripts.State;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class CoursesPage : MonoBehaviour
{
    private bool byLab = true;
    private RegionData region = null;
    private string organ = "";
    private string courseId = "";
    private int labId = 0;

    [Header("Services")] public SpecimenStore store;
    public TrayPage trayPage;
    public StateController stateController;

    [Header("Prefabs")] public SelectorButton selectorPrefab;
    public SelectorButton lightSelectorPrefab;
    public CourseDisplayOptions coursePrefab;
    public LabDisplayOptions labPrefab;
    public SelectorButton noSpecimensPrefab;
    public SelectorButton specimenSelectorPrefab;
    public Button seeAllButtonPrefab;

    [Header("Internal Structures")] public Transform listTransformCourses;
    public Transform listTransformLabs;
    public Text backBttnTitle;
    public Button courseButton;
    public Text selectionTitle;
    public Text noContentText;
    public GameObject labAtlasToggle;
    public Button atlasButton;
    public Button labButton;
    public Text atlasLabel;
    public Text labLabel;
    public Text RecentCourse;
    public Text RecentLab;
    public Text RecentSpecimen;
    public GameObject loadingIndicator;

    private ListMode currentMode;
    private Dictionary<string, SelectorButton> idToButton = new Dictionary<string, SelectorButton>();

    private const string COURSES = "COURSES";
    private const string LABS = "LABS";
    private const string LOADING_SPECIMENS = "LOADING SPECIMENS...";
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

       // selectionTitle.text = LOADING_SPECIMENS;
        noContentText.gameObject.SetActive(false);

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
        //  selectionTitle.text = "";
        ListMode mode = ListMode.LAB_COURSES;
        bool showNoContentText = false;

        if (byLab)
        {
            if (courseId.Length < 1)
            {
                _loadedCourses = store.labCourses.Values.ToList();
                _loadedCourses.Sort((c1, c2) => c1.courseId.CompareTo(c2.courseId)); // sort courses alphebetically
                showNoContentText = _loadedCourses == null || _loadedCourses.Count < 1;
            }
            else if (labId > 0)
            {
                mode = ListMode.LAB_SPECIMENS;
                //  backBttnTitle.text = LABS;
                Tuple<string, List<SpecimenData>> labData = store.GetLabData(courseId, labId);
                selectionTitle.text = labData.Item1;
                _loadedSpecimens = labData.Item2;
                showNoContentText = _loadedSpecimens == null || _loadedSpecimens.Count < 1;
                Debug.Log("labId is here");
            }
            else
            {
                mode = ListMode.LAB;
                //  backBttnTitle.text = COURSES;
                selectionTitle.text = courseId;
                _loadedLabs = store.GetLabDataForCourse(courseId);
                Debug.Log("lab data is here");
                showNoContentText = _loadedLabs == null || _loadedLabs.Count < 1;
                
            }
        }
        else if (region == null)
        {
            mode = ListMode.REGION;
            //  backBttnTitle.text = SHELF;
            _loadedRegions = store.regions.OrderBy(r => r.order).ToList();
            Debug.Log("region is here");
            showNoContentText = _loadedRegions == null || _loadedRegions.Count < 1;
            
        }
        else if (string.IsNullOrEmpty(organ))
        {
            mode = ListMode.REGION_EXPANDED;
            //  backBttnTitle.text = SHELF;
            Debug.Log("expanded is here");
            _loadedRegions = store.regions.OrderBy(r => r.order).ToList();
            _loadedOrgans = store.GetOrgansByRegion(region.name);
        }
        else
        {
            mode = ListMode.SPECIMEN;
            //   backBttnTitle.text = SPECIMEN_LIST;
            selectionTitle.text = organ;
            _loadedOrgans = store.specimensByRegionByOrgan[region.name].Keys.ToList();
            _loadedSpecimens = store.specimensByRegionByOrgan[region.name][organ];
            Debug.Log("specimen is here");
            showNoContentText = _loadedSpecimens == null || _loadedSpecimens.Count < 1;
        }
        Layout(mode, showNoContentText);
    }

    private void Clear()
    {
    //    backButton.onClick.RemoveAllListeners();

        // Clears all menu options
        foreach (Transform child in listTransformCourses)
        {
            Destroy(child.gameObject);
        }
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
      //  SetSpecimenButtonToSelected(specimenId);
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


    /**
     * Lays out UI based on requested mode.
     * Called only through Populate() to ensure that correct data is available.
     */
    private void Layout(ListMode mode, bool showNoContentText)
    {
        loadingIndicator.gameObject.SetActive(store.Loading());
        selectionTitle.gameObject.SetActive(selectionTitle.text.Length > 0);
        noContentText.gameObject.SetActive(showNoContentText);
        /*
        labAtlasToggle.SetActive(
            !store.Loading() &&
            mode != ListMode.SPECIMEN &&
            mode != ListMode.LAB_SPECIMENS
        );
        */

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
            
            foreach (var course in _loadedCourses.Take(3)) //EDIT: Take 3 is for home page currently, whnn needing to iterate over the whole list - remove it 
            {
                CourseDisplayOptions courseOption = Instantiate(coursePrefab, listTransformCourses);
                courseOption.Populate(course.courseId, this);
                Debug.Log("courseId");
            };

         //   courseButton.onClick.AddListener(trayPage.ToggleShelfMenu);
            return;
        }

        if (mode == ListMode.LAB)
        {
            _loadedLabs.ForEach((lab) => {
                LabDisplayOptions labOption = Instantiate(labPrefab, listTransformLabs);
                labOption.Populate(lab, this);
                Debug.Log("labID");
            });

         //   backButton.onClick.AddListener(ClearSelectionData);
            return;
        }
        

        if (mode == ListMode.SPECIMEN || mode == ListMode.LAB_SPECIMENS)
        {
            // Forgive me for the spaghetti below
            // Loops through all loaded specimens of organ type and produces a clickable button for each.
            for (int i = 0; i < _loadedSpecimens.Count; i++)
            {
                string id = _loadedSpecimens[i].id;
                SelectorButton btn = Instantiate(specimenSelectorPrefab, listTransformCourses);
                btn.Populate(_loadedSpecimens[i].name, i, null);
                idToButton.Add(id, btn);
            }

            if (stateController.CurrentSpecimenData != null && trayPage.selectingCompareSpecimen)
            {
                Button btn = Instantiate(seeAllButtonPrefab, listTransformCourses);
                btn.onClick.AddListener(ClearOrganAndLabData);
            }


            // Activates the back button, which takes user back to Region/Organ list
            //  backButton.onClick.AddListener(ClearOrganAndLabData);
           // UpdateSelected();
        }
        else
        {
            // Loops through loaded regions, producing a clickable button for each...
            for (int i = 0; i < _loadedRegions.Count; i++)
            {
                SelectorButton btn = Instantiate(selectorPrefab, listTransformCourses);
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
         //   backButton.onClick.AddListener(trayPage.ToggleShelfMenu);
        }

    }
}