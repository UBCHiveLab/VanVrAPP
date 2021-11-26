using Assets.Scripts.State;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CoursesPage : MonoBehaviour
{
    private bool byLab = true;
    private bool showLab = false;
    private bool showLabSpec = false;
    private bool showSpec = false;
    private RegionData region = null;
    private string organ = "";
    private string courseId = "";
    private int labId = 0;
    private string courseName = "";
    private string labName = "";

    [Header("Services")] public SpecimenStore store;
    public TrayPage trayPage;
    public StateController stateController;

    [Header("Prefabs")] public SelectorButton selectorPrefab;
    public SelectorButton lightSelectorPrefab;
    public CourseDisplayOptions coursePrefab;
    public LabDisplayOptions labPrefab;
    public LabDisplayOptions labTextPrefab;
    public SelectorButton noSpecimensPrefab;
    public SelectorButton specimenPrefab;
    public Button seeAllButtonPrefab;

    [Header("Internal Structures")] public Transform listTransformCourses;
    public Transform listTransformSideCourses;
    public Transform listTransformLabs;
    public Transform listTransformLabText; 
    public Transform listTransformSpec; 
    public Text backBttnTitle;
    public Button courseButton;
    public Button homeButton;
    public Button atlasBtn;
    public Button helpButton;
    public TextMeshProUGUI selectionTitle;
    public Button first;
    public TextMeshProUGUI firstLabel;
    public Button second;
    public TextMeshProUGUI secondLabel;
    public Button third;
    public TextMeshProUGUI thirdLabel;
    public Button fourth;
    public TextMeshProUGUI fourthLabel;
    public Button fifth;
    public TextMeshProUGUI fifthLabel;
     
    public TextMeshPro noContentText;
    public GameObject labAtlasToggle;
    public Button atlasButton;
    public Button labButton;
    public TextMeshProUGUI atlasLabel;
    public TextMeshProUGUI courseLabel;
    public TextMeshProUGUI homeLabel;
    public TextMeshProUGUI helpLabel; 
    public GameObject RecentCourse;
    public GameObject loadingIndicator;
    public GameObject courseInfoContent;
    public GameObject labInfoContent;
    public Button labShowBtn;
    public Button infoShowBtn;
    public Button specimenInfoShowBtn;
    public TextMeshProUGUI coursePageLabLabel;
    public TextMeshProUGUI coursePageInfoLabel;
    public TextMeshProUGUI coursePageSpecLabel; 
    public Button labInfoShowBtn;
    public Button specLabShowBtn; 
    public TextMeshProUGUI labPageSpecLabel;
    public TextMeshProUGUI labPageInfoLabel; 
    public GameObject homeInfo;

    [Header("HomeContentRender")]
    public GameObject welcomePanel;
    public GameObject expandedPanel;
    public GameObject defaultPanel;

    [Header("CourseContentRender")]
    public TextMeshProUGUI courseTitle;
    
    public TextMeshProUGUI labTitle;
    public TextMeshProUGUI labPanel;
    public TextMeshProUGUI courseLabTitle;
    public Button labPanelCourseBtn;
    public RawImage labRenderedImg;
    public GameObject labInfoContentText;

    [Header("LabContentRender")]
    public GameObject sidePanel;
    public Button expandPanelBtn;
    public Sprite expand;
    public Sprite collapse;
    



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
    private ListMode mode;
    private bool showNoContentText; 
    private List<RegionData> _loadedRegions;
    private List<string> _loadedOrgans;
    private List<SpecimenData> _loadedSpecimens;
    private bool _loading = true;
    private List<LabData> _loadedLabs;
    private List<CourseData> _loadedCourses;
    private CourseData _loadedCourse;

    public enum CurrPage
    {
        HOME,
        COURSE,
        LAB,
        HELP
    }

    private CurrPage page;

    void Start()
    {
        if (store == null) store = FindObjectOfType<SpecimenStore>();
        firstLabel.text = "Home";
        secondLabel.text = "";
        thirdLabel.text = "";
        fourthLabel.text = "";
        fifthLabel.text = "";
        BreadCrumbButton();
        // noContentText.gameObject.SetActive(false);
        homeLabel.color = Color.blue; 
        courseButton.onClick.AddListener(ShowCoursesPage);
        homeButton.onClick.AddListener(ShowHomeInfo);
        helpButton.onClick.AddListener(ShowHelpInfo);
        expandPanelBtn.onClick.AddListener(closeSidePanel);
       
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
        mode = ListMode.LAB_COURSES;
        showNoContentText = false;

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
                /*
                mode = ListMode.LAB_SPECIMENS;
                //  backBttnTitle.text = LABS;
                Tuple<string, List<SpecimenData>> labData = store.GetLabData(courseId, labId);
              //  selectionTitle.text = labData.Item1;
                _loadedSpecimens = labData.Item2;
                showNoContentText = _loadedSpecimens == null || _loadedSpecimens.Count < 1;
                Debug.Log("specs are loaded here");
                */
            }
            
            else
            { 
                mode = ListMode.LAB;
                //  backBttnTitle.text = COURSES;
            //    selectionTitle.text = courseId;
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
          //  selectionTitle.text = organ;
            _loadedOrgans = store.specimensByRegionByOrgan[region.name].Keys.ToList();
            _loadedSpecimens = store.specimensByRegionByOrgan[region.name][organ];
            Debug.Log("specimen is here");
            showNoContentText = _loadedSpecimens == null || _loadedSpecimens.Count < 1;
        }
        Layout(mode, showNoContentText);
    }

    private void Clear()
    {
        //  courseButton.onClick.RemoveAllListeners();

        // Clears all menu options
        
        foreach (Transform child in listTransformCourses)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in listTransformSideCourses)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in listTransformLabs)
        {
            Destroy(child.gameObject);
        }
    }

    private void ClearLabText()
    {
        foreach (Transform child in listTransformLabText)
        {
            Destroy(child.gameObject);
        }
    }

    private void ClearLabSpec()
    {
        foreach (Transform child in listTransformSpec)
        {
            Destroy(child.gameObject);
        }
    }

    private void BreadCrumbButton()
    {
        if (secondLabel.text == "> Help")
        {
            second.onClick.AddListener(ShowHelpInfo);
        }
        else if (secondLabel.text == "> Courses")
        {
            second.onClick.AddListener(ShowCoursesPage);
        }
        
    }

    public void CourseSelected(string courseId)
    {
        this.courseId = courseId;
        Populate();
       // var newCourseName = "This Lab is about " + courseId;
        RenderCourseInfo(courseId);
        infoShowBtn.onClick.AddListener(() => RenderCourseInfo(courseId));
        third.onClick.AddListener(() => RenderCourseInfo(courseId));
    }

    private void RenderCourseInfo(string title)
    {
        courseInfoContent.SetActive(true);
        labInfoContent.SetActive(false);
        homeInfo.SetActive(false);
        labInfoContent.SetActive(false);
        
        sidePanel.SetActive(true);
        defaultPanel.SetActive(true);
        expandedPanel.SetActive(false);
        listTransformCourses.GetComponent<GridLayoutGroup>().constraintCount = 3;

        courseTitle.text = title;
        courseName = title;
        coursePageInfoLabel.color = Color.cyan;
        coursePageLabLabel.color = Color.white;
        coursePageSpecLabel.color = Color.white;
        secondLabel.text = "> Courses";
        thirdLabel.text = $"> {courseName}";
        fourthLabel.text = "";
        fifthLabel.text = "";
        // selectionTitle.text = $"Home > Courses > {courseName}";
        //  courseDescription.text = courseDes;
        ShowAllCourses(listTransformSideCourses);
        _loadedLabs = store.GetLabDataForCourse(courseId);
        if (_loadedLabs != null)
        {
            ShowAllLabsText();
        }
       

        // StartCoroutine(LoadLabImg(urlImg));
        labShowBtn.onClick.AddListener(ShowLabDetails);
        specimenInfoShowBtn.onClick.AddListener(ShowSpecimenDetails);

    }

    private void closeSidePanel()
    {
        //expand panel
        if(sidePanel.active){
            sidePanel.SetActive(false);
            expandPanelBtn.transform.GetComponent<Image>().sprite = collapse;
            expandedPanel.SetActive(true);
            defaultPanel.SetActive(false);
            listTransformCourses.GetComponent<GridLayoutGroup>().constraintCount = 5;

            if(page == CurrPage.HOME){
                ShowHomeInfo(5);
            }else if(page == CurrPage.COURSE){
                ShowAllCourses(listTransformCourses);
            }else if(page == CurrPage.HELP){
            }
            
        //collapse panel
        }else{
            sidePanel.SetActive(true);
            expandPanelBtn.transform.GetComponent<Image>().sprite = expand;
            expandedPanel.SetActive(false);
            defaultPanel.SetActive(true);
            listTransformCourses.GetComponent<GridLayoutGroup>().constraintCount = 3;

            if(page == CurrPage.HOME){
                ShowHomeInfo(3);
            }else if(page == CurrPage.COURSE){
                ShowAllCourses(listTransformCourses);
            }
        }
        
    }

    private void ShowSpecimenDetails()
    {
        mode = ListMode.SPECIMEN;
        showSpec = true;
        Layout(mode, showNoContentText);
    }
    private void ShowLabSpecDetails()
    {
        ClearLabSpec();
        mode = ListMode.LAB_SPECIMENS;
        showLabSpec = true;
        labInfoContentText.SetActive(false);
        Tuple<string, List<SpecimenData>> labData = store.GetLabData(courseId, labId);
        //  selectionTitle.text = labData.Item1;
        _loadedSpecimens = labData.Item2;
        showNoContentText = _loadedSpecimens == null || _loadedSpecimens.Count < 1;
        Debug.Log("specs2 are loaded here");
        labPageSpecLabel.color = Color.blue;
        labPageInfoLabel.color = Color.white;
        Layout(mode, showLabSpec);
     //   labInfoShowBtn.onClick.AddListener(() => RenderLabInfo());
    }

    private void ShowLabDetails()
    {
        mode = ListMode.LAB;
        //  backBttnTitle.text = COURSES;
        //    selectionTitle.text = courseId;
        
        sidePanel.SetActive(true);
        defaultPanel.SetActive(true);
        expandedPanel.SetActive(false);
        listTransformCourses.GetComponent<GridLayoutGroup>().constraintCount = 3;

        _loadedLabs = store.GetLabDataForCourse(courseId);
        Debug.Log("lab data is here");
        showNoContentText = _loadedLabs == null || _loadedLabs.Count < 1;
        Layout(mode, showNoContentText);
        coursePageInfoLabel.color = Color.white;
        coursePageLabLabel.color = Color.blue;
        coursePageSpecLabel.color = Color.white;
    }

    public void LabSelected(int labId, String labName, String labImg)
    {
        this.labId = labId;
        Populate();
       // var newLabName = "This Lab is about " + labName;
        RenderLabInfo(labName, labId, labImg);
        fourth.onClick.AddListener(() => RenderLabInfo(labName, labId, labImg));
   
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
        //    UpdateSelected();
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
          //  OnCloseLabInfo();
            btn.SetLoadingUntil(() => store.specimens[_loadedSpecimens[btn.indexValue].id].dataLoaded);
       //     UpdateSelected();
        });
    }

    private void RenderLabInfo(String title, int labId, String urlImg)
    {
        
        homeInfo.SetActive(false);
        courseInfoContent.SetActive(false);
        labTitle.text = title;
        labPanel.text = $"Lab {labId}";
        labName = $"Lab {labId}";
        courseLabTitle.text = courseName;
        labInfoContentText.SetActive(true);
        ClearLabSpec();
        //  labDescription.text = labDes;
        labInfoContent.SetActive(true);
        ShowAllLabs();
        fourthLabel.text = $"> {labName}";
        fifthLabel.text = "";
        //  selectionTitle.text = $"Home > Courses > {courseName} > {labName}";
        labPanelCourseBtn.onClick.AddListener(() => CourseSelected(courseName));
        specLabShowBtn.onClick.AddListener(ShowLabSpecDetails);
        labInfoShowBtn.onClick.AddListener(() => RenderLabInfo(title, labId, urlImg));
        labPageInfoLabel.color = Color.blue;
        labPageSpecLabel.color = Color.white;
      //  StartCoroutine(LoadLabImg(urlImg));
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
      //  selectionTitle.gameObject.SetActive(selectionTitle.text.Length > 0);
      //  noContentText.gameObject.SetActive(showNoContentText);
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
            
            foreach (var course in _loadedCourses.Take(3)) 
            {
                CourseDisplayOptions courseOption = Instantiate(coursePrefab, listTransformCourses);
                courseOption.Populate(course, this);
               
            };

            //   courseButton.onClick.AddListener(trayPage.ToggleShelfMenu);
            return;
        }
        
        
        if (mode == ListMode.LAB)
        {
        _loadedLabs.ForEach((lab) => {
                LabDisplayOptions labOption = Instantiate(labPrefab, listTransformLabs);
                labOption.Populate(lab, this);
            });
            

         //   backButton.onClick.AddListener(ClearSelectionData);
            return;
        }
        
        
        if ((mode == ListMode.SPECIMEN) && (showSpec == true))
        {
            // Forgive me for the spaghetti below
            // Loops through all loaded specimens of organ type and produces a clickable button for each.

         //   backButton.transform.GetChild(0).GetComponent<Image>().sprite = shelfBack;

            for (int i = 0; i < _loadedSpecimens.Count; i++)
            {
                string id = _loadedSpecimens[i].id;
                SelectorButton btn = Instantiate(specimenPrefab, listTransformSpec);
                btn.Populate(_loadedSpecimens[i].name, i, null);
                idToButton.Add(id, btn);
            }

            if (stateController.CurrentSpecimenData != null && trayPage.selectingCompareSpecimen)
            {
                Button btn = Instantiate(seeAllButtonPrefab, listTransformSpec);
                btn.onClick.AddListener(ClearOrganAndLabData);
            }

            // Activates the back button, which takes user back to Region/Organ list
        //    backButton.onClick.AddListener(ClearOrganAndLabData);
        //    UpdateSelected();
        }

        else if (mode == ListMode.LAB_SPECIMENS)
        {
            // Same as specimen mode, also provide additional content info rendering
         //   backButton.transform.GetChild(0).GetComponent<Image>().sprite = shelfBack;

            for (int i = 0; i < _loadedSpecimens.Count; i++)
            {
                string id = _loadedSpecimens[i].id;
                SelectorButton btn = Instantiate(specimenPrefab, listTransformSpec);
                Debug.Log("btn used");
                btn.Populate(_loadedSpecimens[i].name, i, null);
                idToButton.Add(id, btn);
            }

            if (stateController.CurrentSpecimenData != null && trayPage.selectingCompareSpecimen)
            {
                Button btn = Instantiate(seeAllButtonPrefab, listTransformSpec);
                btn.onClick.AddListener(ClearOrganAndLabData);
            }

            // Activates the back button, which takes user back to Region/Organ list
        //    backButton.onClick.AddListener(ClearOrganAndLabData);
        //    backButton.onClick.AddListener(() => {
          //    labInfoContent.SetActive(false);
            //  labInfoShowBtn.SetActive(false);
         //   });

        //    UpdateSelected();
        }
    
        /*
        else
        {
            backButton.transform.GetChild(0).GetComponent<Image>().sprite = shelfClose;
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
          //  backButton.onClick.AddListener(trayPage.ToggleShelfMenu);
        }
        */
    }

    
    

    private void ShowCoursesPage()
    {
        Clear();
        ShowAllCourses(listTransformCourses);
        homeInfo.SetActive(true);
        courseInfoContent.SetActive(false);
        RecentCourse.SetActive(false);
        welcomePanel.SetActive(false);
        labInfoContent.SetActive(false);
        secondLabel.text = "> Courses";
        thirdLabel.text = "";
        fourthLabel.text = "";
        fifthLabel.text = "";
        //  selectionTitle.text = "Home > Courses";
        homeLabel.color = Color.black;
        atlasLabel.color = Color.black;
        helpLabel.color = Color.black;
        courseLabel.color = Color.blue;
        page = CurrPage.COURSE;
        showSpec = false;
        showLabSpec = false;
    }

    public void ShowHomeInfo()
    {
        if(sidePanel.active){
            ShowHomeInfo(3);
        } else{
            ShowHomeInfo(5);
        }
    }

    public void ShowHomeInfo(int num)
    {
        Clear();
        foreach (var course in _loadedCourses.Take(num))
        {
            CourseDisplayOptions courseOption = Instantiate(coursePrefab, listTransformCourses);
            courseOption.Populate(course, this);
           
        };
        homeInfo.SetActive(true);
        courseInfoContent.SetActive(false);
        RecentCourse.SetActive(true);
        welcomePanel.SetActive(true);
        labInfoContent.SetActive(false);
        homeLabel.color = Color.blue;
        atlasLabel.color = Color.black;
        helpLabel.color = Color.black;
        courseLabel.color = Color.black;
      //  selectionTitle.text = "Home";
        listTransformCourses.GetComponent<GridLayoutGroup>().constraintCount = num;

        page = CurrPage.HOME;
        showSpec = false;
        showLabSpec = false;
    }

    public void ShowHelpInfo()
    {
        Clear();
        homeInfo.SetActive(true);
        courseInfoContent.SetActive(false);
        RecentCourse.SetActive(false);
        welcomePanel.SetActive(false);
        labInfoContent.SetActive(false);
        secondLabel.text = "> Help";
        thirdLabel.text = "";
        fourthLabel.text = "";
        fifthLabel.text = "";
     //   selectionTitle.text = "Home > Help";
        homeLabel.color = Color.black;
        atlasLabel.color = Color.black;
        helpLabel.color = Color.blue;
        courseLabel.color = Color.black;

        page = CurrPage.HELP;
        showSpec = false;
        showLabSpec = false;

    }

    private void ShowAllLabs()
    {
        Clear();
        _loadedLabs.ForEach((lab) => {
            LabDisplayOptions labOption = Instantiate(labPrefab, listTransformLabs);
            labOption.Populate(lab, this);
        });
    }

    private void ShowAllCourses(Transform listTransform)
    {
        Clear();
        foreach (var course in _loadedCourses)
        {
            CourseDisplayOptions courseOption = Instantiate(coursePrefab, listTransform);
            courseOption.Populate(course, this);
          
        };
    }

    private void ShowAllLabsText()
    {
        ClearLabText();
        _loadedLabs.ForEach((lab) =>
        {
            LabDisplayOptions labOption = Instantiate(labTextPrefab, listTransformLabText);
            labOption.Populate(lab, this);
        });
    }

}