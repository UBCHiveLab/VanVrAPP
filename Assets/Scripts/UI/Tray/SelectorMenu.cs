using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Assets.Scripts.State;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

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
    //public TextMeshProUGUI backBttnTitle;
    public Button backButton;
    public TextMeshProUGUI selectionTitle;
    public TextMeshProUGUI noContentText;
    public GameObject labAtlasToggle;
    public Button atlasButton;
    public Button labButton;
    public TextMeshProUGUI atlasLabel;
    public TextMeshProUGUI labLabel;
    public TextMeshProUGUI titleOnShelf;
    public GameObject loadingIndicator;
    public Animator anim;
    public GameObject labInfoContent;
    public GameObject labInfoShowBtn;

    private ListMode currentMode;
    private Dictionary<string, SelectorButton> idToButton = new Dictionary<string, SelectorButton>();

    private const string COURSES = "COURSES";
    private const string LABS = "LABS";
    private const string LOADING_SPECIMENS = "LOADING SPECIMENS...";
    private const string SHELF = "SHELF";
    private const string SPECIMEN_LIST = "SPECIMEN LIST";

    [Header("LabContentRender")]
    public TextMeshProUGUI shelfTitle;
    public TextMeshProUGUI labDescription;
    public RawImage labRenderedImg;

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

        selectionTitle.text = LOADING_SPECIMENS;
        noContentText.gameObject.SetActive(false);
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
        bool showNoContentText = false;

        // Sets current list mode based on set fields.
        // Then prepares requested data:
        if (store.Loading())
        {
            // do nothing, as the data used to populate the UI hasn't loaded yet
            selectionTitle.text = LOADING_SPECIMENS;
        } else if (byLab)
        {
            if (courseId.Length < 1)
            {
                //backBttnTitle.text = SHELF;
                _loadedCourses = store.labCourses.Values.ToList();
                _loadedCourses.Sort((c1, c2) => c1.courseId.CompareTo(c2.courseId)); // sort courses alphebetically
                showNoContentText = _loadedCourses == null || _loadedCourses.Count < 1;
            }
            else if (labId > 0)
            {
                mode = ListMode.LAB_SPECIMENS;
                //backBttnTitle.text = LABS;
                Tuple<string, List<SpecimenData>> labData = store.GetLabData(courseId, labId);
                selectionTitle.text = labData.Item1;
                _loadedSpecimens = labData.Item2;
                showNoContentText = _loadedSpecimens == null || _loadedSpecimens.Count < 1;
            }
            else
            {
                mode = ListMode.LAB;
                //backBttnTitle.text = COURSES;
                selectionTitle.text = courseId;
                _loadedLabs = store.GetLabDataForCourse(courseId);
                showNoContentText = _loadedLabs == null || _loadedLabs.Count < 1;
            }
        }
        else if (region == null)
        {
            mode = ListMode.REGION;
            //backBttnTitle.text = SHELF;
            _loadedRegions = store.regions.OrderBy(r => r.order).ToList();
            showNoContentText = _loadedRegions == null || _loadedRegions.Count < 1;
        }
        else if (string.IsNullOrEmpty(organ))
        {
            mode = ListMode.REGION_EXPANDED;
            //backBttnTitle.text = SHELF;
            _loadedRegions = store.regions.OrderBy(r => r.order).ToList();
            _loadedOrgans = store.GetOrgansByRegion(region.name);
        }
        else
        {
            mode = ListMode.SPECIMEN;
            //backBttnTitle.text = SPECIMEN_LIST;
            selectionTitle.text = organ;
            _loadedOrgans = store.specimensByRegionByOrgan[region.name].Keys.ToList();
            _loadedSpecimens = store.specimensByRegionByOrgan[region.name][organ];
            showNoContentText = _loadedSpecimens == null || _loadedSpecimens.Count < 1;
        }

        Layout(mode, showNoContentText);
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

    public void LabSelected(int labId, String labName,String labImg)
    {
        this.labId = labId;
        Populate();
        var newLabName = "This Lab is about " + labName;
        RenderLabInfo(labName, newLabName,labImg);
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
            _loadedLabs.ForEach((lab) => {
                LabOption labOption = Instantiate(labPrefab, listTransform);
                labOption.Populate(lab, this);
            });

            backButton.onClick.AddListener(ClearSelectionData);
            labInfoShowBtn.SetActive(false);
            return;
        }

        if (mode == ListMode.SPECIMEN)
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
        else if(mode == ListMode.LAB_SPECIMENS)
        {
            // Same as specimen mode, also provide additional content info rendering
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
        atlasLabel.color = Color.blue;
        labLabel.color = Color.white;
        titleOnShelf.gameObject.SetActive(false);
        titleOnShelf.text = "ANATOMICAL CATEGORIES";
        titleOnShelf.gameObject.SetActive(true);
        byLab = false;
        ClearSelectionData();
    }

    private void ToggleToLabs()
    {
        labButton.interactable = false;
        atlasButton.interactable = true;
        labLabel.color = Color.blue;
        atlasLabel.color = Color.white;
        titleOnShelf.gameObject.SetActive(false);
        titleOnShelf.text = "LAB COURSES";
        titleOnShelf.gameObject.SetActive(true);
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
            OnCloseLabInfo();
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

    public void LoadSpecimenFromInfo()
    {
        Debug.Log("Loaded Specimen");
        UpdateSelected();
    }

    public void RenderLabInfo(String title, String labDes, String urlImg)
    {
        labInfoContent.SetActive(false);
        labInfoShowBtn.SetActive(false);
        shelfTitle.text = title;
        labDescription.text = labDes;
        StartCoroutine(LoadLabImg(urlImg));
        labInfoContent.SetActive(true);
    }

    private IEnumerator LoadLabImg(String url)
    {
        url = url.Trim();

        if (url.Length > 0)
        {
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
            yield return request.SendWebRequest();
            if (request.isNetworkError || request.isHttpError)
                Debug.Log(request.error);

            else
                labRenderedImg.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
        }
        else
        {
            url = "https://www.salonlfc.com/wp-content/uploads/2018/01/image-not-found-scaled.png";
            url = url.Trim();
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
            yield return request.SendWebRequest();
            if (request.isNetworkError || request.isHttpError)
                Debug.Log(request.error);

            else
                labRenderedImg.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
        }
    }

    public void OnCloseLabInfo()
    {
        labInfoContent.SetActive(false);
        labInfoShowBtn.SetActive(true);
    }

    public void OnShowLabInfo()
    {
        labInfoContent.SetActive(true);
        labInfoShowBtn.SetActive(false);
    }
}