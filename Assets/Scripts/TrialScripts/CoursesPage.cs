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
    
    private RegionData region = null;
    private string organ = "";
    private string courseId = "";
    private int labId = 0;
    private string courseName = "";
    private string labName = "";

    [Header("Services")] public SpecimenStore store;
    public TrayPage trayPage;
    public StateController stateController;
    public SelectorMenu selectorMenu;
    public AnalysisPage analysisPage; 

    [Header("Prefabs")] public SelectorButton selectorPrefab;
    public SelectorButton lightSelectorPrefab;
    public CourseDisplayOptions coursePrefab;
    public LabDisplayOptions labPrefab;
    public LabDisplayOptions labTextPrefab;
    public TextMeshProUGUI labNamePrefab;
    public LabSpecDisplayOptions labSpecDisplayPrefab;
    public SelectorButton noSpecimensPrefab;
    public SelectorButton specimenPrefab;
    public Button seeAllButtonPrefab;

    [Header("Internal Structures")] public Transform listTransformCourses;
    public Transform listTransformAllCourses; 
    public Transform listTransformSideCourses;
    public Transform listTransformLabs;
    public Transform listTransformLabText; 
    public Transform listTransformCourseSpec;
    public Transform listTransformSpec; 
    public Transform listTransformLabName;
    public Transform listTransformTab; 
    public Transform listTransformChildren; 
    public Text backBttnTitle;
    public TextMeshProUGUI noContentText;
    public GameObject RecentCourse;

    [Header("BreadCrumb")]
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

    [Header("SideButtons")]
    public Button courseButton;
    public TextMeshProUGUI courseLabel;
    public Button homeButton;
    public TextMeshProUGUI homeLabel;
    public Button atlasBtn;
    public TextMeshProUGUI atlasLabel;
    public Button helpButton;
    public TextMeshProUGUI helpLabel;

    [Header("HomeContentRender")]
    public GameObject homeInfo;
    public GameObject welcomePanel;
    public GameObject expandedPanel;
    public GameObject defaultPanel;
    public ScrollRect homeScrollRect;
    public Button expandPanelBtn;
    public Sprite expand;
    public Sprite collapse;

    [Header("CourseContentRender")]
    public TextMeshProUGUI courseTitle;
    public GameObject courseInfoContent;
    public GameObject courseInfoContentText;
    public GameObject labInfoContentText;
    public ScrollRect courseScrollRect;
    public Transform courseContent; 
    public TextMeshProUGUI labSpecName;
    //***
    public Button labShowBtn;
    public Button infoShowBtn;
    public Button specimenInfoShowBtn;
    public TextMeshProUGUI coursePageLabLabel;
    public TextMeshProUGUI coursePageInfoLabel;
    public TextMeshProUGUI coursePageSpecLabel; 


    [Header("LabContentRender")]
     public GameObject labInfoContent;
    public TextMeshProUGUI labTitle;
    public TextMeshProUGUI labPanel;
    public TextMeshProUGUI courseLabTitle;
    public Button labPanelCourseBtn;
    public Button labInfoShowBtn;
    public Button specLabShowBtn; 
    public TextMeshProUGUI labPageSpecLabel;
    public TextMeshProUGUI labPageInfoLabel; 
    public ScrollRect labScrollRect;
    public RawImage specimenRenderedImg;
   
    public GameObject SpecimenLoadingPopUpScreen;

    [Header("SidePanel")]
    public Button previewBtn; 
    public Image previewImage; 
    public TextMeshProUGUI specimenText; 
    public TextMeshProUGUI previewText;
    public GameObject sidePanel;
    public ScrollRect sidePanelScrollRect;

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
        LAB_SPECIMENS,
        COURSE_SPECIMENS
    }
    private ListMode mode;
    private bool showNoContentText; 
    private List<RegionData> _loadedRegions;
    private List<string> _loadedOrgans;
    private List<SpecimenData> _loadedSpecimens;
    private List<SpecimenData> _loadedCourseSpecimens;
    
    private List<LabData> _loadedCourseLabs; 
    private bool _loading = true;
    private List<LabData> _loadedLabs;
    private List<CourseData> _loadedCourses;
    private CourseData _loadedCourse;

    private Color hiveBlue = new Color(18/255f,143/255f,191/255f);
    private Color defaultBlue = new Color(0/255f,11/255f,63/255f);

    public enum CurrPage
    {
        HOME,
        COURSE,
        LAB,
        HELP
    }

    private CurrPage page;

    /*
    Determines the functionality of the different buttons
    */
    void Start()
    {
        if (store == null) store = FindObjectOfType<SpecimenStore>();
        
        UpdateUI();
        
        homeScrollRect.verticalNormalizedPosition = 1.5f;
        homeLabel.color = hiveBlue; 
        courseButton.onClick.AddListener(ShowCoursesPage);
        courseButton.onClick.AddListener(() => {selectorMenu.ToggleToLabs(); });

        homeButton.onClick.AddListener(ShowHomeInfo);
        homeButton.onClick.AddListener(() => {selectorMenu.ToggleToLabs(); });

        helpButton.onClick.AddListener(ShowHelpInfo);
        atlasBtn.onClick.AddListener(ShowAtlasInfo);
        atlasBtn.onClick.AddListener(() => {selectorMenu.ToggleToAtlas(); });
        expandPanelBtn.onClick.AddListener(closeSidePanel);
       
    }

    private void UpdateUI()
    {
        firstLabel.text = "Home";
        secondLabel.text = "";
        thirdLabel.text = "";
        fourthLabel.text = "";
        fifthLabel.text = "";
        
        first.onClick.AddListener(ShowHomeInfo);
        first.onClick.AddListener(() => {selectorMenu.ToggleToLabs(); });
       
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
                _loadedLabs = store.GetLabDataForCourse(courseId);
              //  Debug.Log("lab data is here");
                showNoContentText = _loadedLabs == null || _loadedLabs.Count < 1;   
            }
            
        }
        else if (region == null)
        {
            mode = ListMode.REGION;
            _loadedRegions = store.regions.OrderBy(r => r.order).ToList();
            Debug.Log("region is here");
            showNoContentText = _loadedRegions == null || _loadedRegions.Count < 1;

        }
        else if (string.IsNullOrEmpty(organ))
        {
            mode = ListMode.REGION_EXPANDED;
            Debug.Log("expanded is here");
            _loadedRegions = store.regions.OrderBy(r => r.order).ToList();
            _loadedOrgans = store.GetOrgansByRegion(region.name);
        }
        else
        {
            mode = ListMode.SPECIMEN;
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

        foreach (Transform child in listTransformAllCourses)
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
        foreach (Transform child in listTransformTab)
        {
            Destroy (child.gameObject);
        }
        foreach (Transform child in listTransformChildren)
        {
            Destroy (child.gameObject); 
        }
    }

    private void ClearLabText()
    {
        foreach (Transform child in listTransformLabText)
        {
            Destroy(child.gameObject);
        }
    }

    private void ClearSpec()
    {
        foreach (Transform child in listTransformSpec)
        {
            Destroy(child.gameObject);
        }      
    }

    private void ClearLabSpec()
    {
        foreach (Transform child in listTransformLabName)
        {
            Destroy(child.gameObject);
        }
        
    }
    

    public void CourseSelected(string courseId)
    {
        this.courseId = courseId;
        Populate();
        RenderCourseInfo(courseId);
        infoShowBtn.onClick.AddListener(() => RenderCourseInfo(courseId));
        infoShowBtn.onClick.AddListener(() => SidePanelPreviewOff()); 
        if (secondLabel.text == "> Courses")
        {
            second.onClick.AddListener(ShowCoursesPage);
            second.onClick.AddListener(() => {selectorMenu.ToggleToLabs(); });
        }
        third.onClick.AddListener(() => RenderCourseInfo(courseId));
      //  third.onClick.AddListener(() => {selectorMenu.RenderCourseInfo(courseId); });

        
    }

    private void RenderCourseInfo(string title)
    {
        SidePanelPreviewOff(); 
        ClearLabSpec();
        courseInfoContent.SetActive(true);
        courseInfoContentText.SetActive(true);

        labInfoContent.SetActive(false);
        homeInfo.SetActive(false);
        labInfoContent.SetActive(false);
        labSpecDisplayPrefab.gameObject.SetActive(false);
        courseScrollRect.verticalNormalizedPosition = 1.5f;

        sidePanel.SetActive(true);
        defaultPanel.SetActive(true);
        expandedPanel.SetActive(false);
        listTransformCourses.GetComponent<GridLayoutGroup>().constraintCount = 3;
        LayoutElement courseLayoutElement = courseContent.GetComponent<LayoutElement>(); 
         
        courseTitle.text = title;
        courseName = title;
        coursePageInfoLabel.color = hiveBlue;
        coursePageLabLabel.color = defaultBlue;
        coursePageSpecLabel.color = defaultBlue;
        secondLabel.text = "> Courses";
        thirdLabel.text = $"> {courseName}";
        fourthLabel.text = "";
        fifthLabel.text = "";
        
        ShowAllCourses(listTransformSideCourses);
        _loadedLabs = store.GetLabDataForCourse(courseId);
     //   Debug.Log(_loadedLabs.Count());

        ShowAllLabsText();

        Tuple<string, List<int>, List<SpecimenData>> courseSpecData = store.GetCourseData(courseId);
        
        _loadedCourseSpecimens = courseSpecData.Item3;
        if (_loadedCourseSpecimens.Count() == 0)
        {
            specimenInfoShowBtn.interactable = false;
        }
        else 
        {
            specimenInfoShowBtn.interactable = true;
        }
        labShowBtn.onClick.AddListener(ShowLabDetails);
        labShowBtn.onClick.AddListener(() => SidePanelPreviewOff());
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
                ShowAllCourses(listTransformAllCourses);
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
                ShowAllCourses(listTransformAllCourses);
            }
        }
        
    }

    private void ShowSpecimenDetails()
    {
        TextMeshProUGUI textObject;
        ClearLabSpec();
        mode = ListMode.COURSE_SPECIMENS;
        
        courseInfoContentText.SetActive(false);
        courseScrollRect.verticalNormalizedPosition = 1.5f;
        coursePageInfoLabel.color = defaultBlue;
        coursePageLabLabel.color = defaultBlue;
        coursePageSpecLabel.color = hiveBlue;
        labSpecDisplayPrefab.gameObject.SetActive(true);
        
        List<Tuple<int, List<SpecimenData>>> courseSpecData = store.GetSpecimenData(courseId);
        foreach (Tuple<int, List<SpecimenData>> tuple in courseSpecData)
        {
            labSpecDisplayPrefab.gameObject.SetActive(true); 
            textObject = labSpecDisplayPrefab.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
            textObject.text = $"Lab {tuple.Item1.ToString()}";
            LabSpecDisplayOptions options = Instantiate(labSpecDisplayPrefab, listTransformLabName);
            _loadedCourseSpecimens = tuple.Item2;
           
            if (_loadedCourseSpecimens.Count() > 12)
            {
                listTransformLabName.GetComponent<VerticalLayoutGroup>().spacing = 170;
                options.children.GetComponent<GridLayoutGroup>().padding.top = 50;     
            } else if ((_loadedCourseSpecimens.Count() > 8) && (_loadedCourseSpecimens.Count() <= 12))
            {
                listTransformLabName.GetComponent<VerticalLayoutGroup>().spacing = 100; 
                options.children.GetComponent<GridLayoutGroup>().padding.top = 0; 
            } else if ((_loadedCourseSpecimens.Count() > 4) && (_loadedCourseSpecimens.Count() <= 8))
            {
                listTransformLabName.GetComponent<VerticalLayoutGroup>().spacing = 140; 
                options.children.GetComponent<GridLayoutGroup>().padding.top = -60;
            }
            else if ((_loadedCourseSpecimens.Count() <= 4))
            {
                listTransformLabName.GetComponent<VerticalLayoutGroup>().spacing = 100; 
                options.children.GetComponent<GridLayoutGroup>().padding.top = -100;
            }

            for (int i = 0; i < _loadedCourseSpecimens.Count; i++)
            {
                string id = _loadedCourseSpecimens[i].id;
                string imgUrl = _loadedCourseSpecimens[i].imgUrl; 
                SelectorButton btn = Instantiate(lightSelectorPrefab, options.children);
                btn.Populate(_loadedCourseSpecimens[i].name, i, null);
                btn.button.onClick.AddListener(() => SidePanelPreview(id, imgUrl));
            }
            
        }  
        
        
        
    }
    private void ShowLabSpecDetails()
    {
        ClearSpec();
        mode = ListMode.LAB_SPECIMENS;
        labInfoContentText.SetActive(false);
        
        Tuple<string, List<SpecimenData>> labData = store.GetLabData(courseId, labId);
        _loadedSpecimens = labData.Item2;
        showNoContentText = _loadedSpecimens == null || _loadedSpecimens.Count < 1;
        labPageSpecLabel.color = hiveBlue;
        labPageInfoLabel.color = defaultBlue;
        Layout(mode, showNoContentText);
    }

    private void ShowLabDetails()
    {
        mode = ListMode.LAB;
        sidePanel.SetActive(true);
        defaultPanel.SetActive(true);
        expandedPanel.SetActive(false);
        listTransformCourses.GetComponent<GridLayoutGroup>().constraintCount = 3;

        _loadedLabs = store.GetLabDataForCourse(courseId);
      //  Debug.Log("lab data is here");
        showNoContentText = _loadedLabs == null || _loadedLabs.Count < 1;
        Layout(mode, showNoContentText);

        coursePageInfoLabel.color = defaultBlue;
        coursePageLabLabel.color = hiveBlue;
        coursePageSpecLabel.color = defaultBlue;
        
    }

    public void LabSelected(int labId, String labName, String labImg)
    {
        this.labId = labId;
        Populate();
       var newLabName = "This Lab is about " + labName;
        RenderLabInfo(labName, labId, labImg);
        fourth.onClick.AddListener(() => RenderLabInfo(labName, labId, labImg));
        fourth.onClick.AddListener(() => {selectorMenu.RenderLabInfo(labName, newLabName, labImg); });
   
    }

    private void RenderLabInfo(String title, int labId, String urlImg)
    {
        SidePanelPreviewOff(); 
        homeInfo.SetActive(false);
        courseInfoContent.SetActive(false);
        labTitle.text = title;
        labPanel.text = $"Lab {labId}";
        labName = $"Lab {labId}";
        courseLabTitle.text = courseName;
        labInfoContentText.SetActive(true);
        labScrollRect.verticalNormalizedPosition = 1.5f;
        ClearSpec();
        trayPage.SetActionOff();
        labInfoContent.SetActive(true);
        ShowAllLabs();
        fourthLabel.text = $"> {labName}";
        fifthLabel.text = "";
        third.onClick.AddListener(() => CourseSelected(courseName));
        labPanelCourseBtn.onClick.AddListener(() => CourseSelected(courseName));
        labPanelCourseBtn.onClick.AddListener(() => selectorMenu.CourseSelected(courseName));
        labPanelCourseBtn.onClick.AddListener(() => selectorMenu.ClearOrganAndLabData());
        labPanelCourseBtn.onClick.AddListener(()=> {selectorMenu.labInfoContent.SetActive(false); });

        specLabShowBtn.onClick.AddListener(ShowLabSpecDetails);
        labInfoShowBtn.onClick.AddListener(() => RenderLabInfo(title, labId, urlImg));
        labInfoShowBtn.onClick.AddListener(() => SidePanelPreviewOff());
        labPageInfoLabel.color = hiveBlue;
        labPageSpecLabel.color = defaultBlue;
    }

    private IEnumerator LoadSpecimenImg(String url)
    {
        // url = url.Trim();

        // if (url.Length > 0)
        // {
        //     UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        //     yield return request.SendWebRequest();
        //     if (request.isNetworkError || request.isHttpError)
        //         Debug.Log(request.error);

        //     else
        //         specimenRenderedImg.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
        // }
        // else
        // {
        
            url = "https://www.salonlfc.com/wp-content/uploads/2018/01/image-not-found-scaled.png";
            url = url.Trim();
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
            yield return request.SendWebRequest();
            if (request.isNetworkError || request.isHttpError)
                Debug.Log(request.error);

            else
                specimenRenderedImg.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                
      //  }
        
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

    private void ClearSelectionData()
    {
        courseId = "";
        region = null;
        ClearOrganAndLabData();
    }


    /**
     * Lays out UI based on requested mode.
     * Called only through Populate() to ensure that correct data is available.
     */
    public void Layout(ListMode mode, bool showNoContentText)
    {
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
            
            foreach (var course in _loadedCourses.Take(3 * 2)) 
            {
                CourseDisplayOptions courseOption = Instantiate(coursePrefab, listTransformCourses);
                courseOption.Populate(course, this, selectorMenu);
               
            };

            //   courseButton.onClick.AddListener(trayPage.ToggleShelfMenu);
            return;
        }
        
        
        if (mode == ListMode.LAB)
        {
            if (_loadedLabs.Count() == 0)
        {
            labShowBtn.interactable = false;
        }
        else 
        {
            labShowBtn.interactable = true;
             _loadedLabs.ForEach((lab) => {
                LabDisplayOptions labOption = Instantiate(labPrefab, listTransformLabs);
                labOption.Populate(lab, this, selectorMenu);
            });
        }
      
         //   backButton.onClick.AddListener(ClearSelectionData);
            return;
        }
        
        
        if (mode == ListMode.SPECIMEN) 
        {
            if (_loadedSpecimens.Count() == 0)
            {
                Debug.Log("No Specimens found here");
                specimenInfoShowBtn.interactable = false;
            }
            else {
                specimenInfoShowBtn.interactable = true;
            } 
            // Forgive me for the spaghetti below
            // Loops through all loaded specimens of organ type and produces a clickable button for each.
                  
            for (int i = 0; i < _loadedSpecimens.Count; i++)
            {
                string id = _loadedSpecimens[i].id;
                string imgUrl = _loadedSpecimens[i].imgUrl; 

                if (secondLabel.text == "> 3D Atlas")
                {
                    SidePanelPreview(id, imgUrl);
                    
                }
                else 
                {
                    SelectorButton btn = Instantiate(lightSelectorPrefab, listTransformSpec);
                    btn.Populate(_loadedSpecimens[i].name, i, null);
                    btn.button.onClick.AddListener(() => SidePanelPreview(id, imgUrl));   
                    idToButton.Add(id, btn);
                }        
            }

            if (stateController.CurrentSpecimenData != null && trayPage.selectingCompareSpecimen)
            {
                Button btn = Instantiate(seeAllButtonPrefab, listTransformSpec);
                btn.onClick.AddListener(ClearOrganAndLabData);
            }
            
            selectorMenu.UpdateSelected();
        }
        else if (mode == ListMode.COURSE_SPECIMENS)
        {

            if (_loadedCourseSpecimens.Count() == 0)
            {
                Debug.Log("No Specimens found here");
                specimenInfoShowBtn.interactable = false;
            }
            else 
            {
                specimenInfoShowBtn.interactable = true;
                for (int i = 0; i < _loadedCourseSpecimens.Count; i++)
                {
                    string id = _loadedCourseSpecimens[i].id;
                    string imgUrl = _loadedCourseSpecimens[i].imgUrl; 
                    SelectorButton btn = Instantiate(lightSelectorPrefab, listTransformCourseSpec);
                    btn.Populate(_loadedCourseSpecimens[i].name, i, null);
                    btn.button.onClick.AddListener(() => SidePanelPreview(id, imgUrl));
                    btn.button.onClick.AddListener(() => selectorMenu.SelectSpecimen(id));     
                    idToButton.Add(id, btn);
                  //  Debug.Log("course specimen reached layout room");
                }
            }   
           

            if (stateController.CurrentSpecimenData != null && trayPage.selectingCompareSpecimen)
            {
                Button btn = Instantiate(seeAllButtonPrefab, listTransformCourseSpec);
                btn.onClick.AddListener(ClearOrganAndLabData);
            }
 
        }

        else if (mode == ListMode.LAB_SPECIMENS)
        {
            if (_loadedSpecimens.Count() == 0)
            {
                Debug.Log("No Specimens found here");
                specimenInfoShowBtn.interactable = false;
            }
            else {
                specimenInfoShowBtn.interactable = true;
            } 
            // Same as specimen mode, also provide additional content info rendering
         //   backButton.transform.GetChild(0).GetComponent<Image>().sprite = shelfBack;

            for (int i = 0; i < _loadedSpecimens.Count; i++)
            {
                string id = _loadedSpecimens[i].id;
                string imgUrl = _loadedSpecimens[i].imgUrl; 
                SelectorButton btn = Instantiate(lightSelectorPrefab, listTransformSpec);
                btn.Populate(_loadedSpecimens[i].name, i, null);
                idToButton.Add(id, btn);
                btn.button.onClick.AddListener(() => SidePanelPreview(id, imgUrl));    
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
    
        
        else
        {
           // backButton.transform.GetChild(0).GetComponent<Image>().sprite = shelfClose;
            // Loops through loaded regions, producing a clickable button for each...
            for (int i = 0; i < _loadedRegions.Count; i++)
            {
                SelectorButton btn = Instantiate(selectorPrefab, listTransformTab);
                btn.Populate(_loadedRegions[i].name, i, _loadedRegions[i].icon);  
                idToButton.Add(_loadedRegions[i].name, btn);

                // If a region is the currently selected, output the organs found as buttons below.
                if (_loadedRegions[i] == region)
                {
                    AdjustRegionLayout(btn, i); 
                    Debug.Log("region is loaded region"); 
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
                          //  sbtn.button.onClick.AddListener(() => {SelectSpecimen(_loadedOrgans[sbtn.indexValue]); });
                            sbtn.button.onClick.AddListener(() => {SelectOrgan(_loadedOrgans[sbtn.indexValue]); });
                            idToButton.Add(_loadedOrgans[sbtn.indexValue], sbtn);       
                        }
                    }

                    // Bind a click listener that closes the region accordion
                    btn.button.onClick.AddListener(UnselectRegion);
                    btn.button.onClick.AddListener(ResetRegionLayout); 
                }
                else
                {
                    btn.children.gameObject.SetActive(false);
                    // Bind a click listener that closes the current region accordion and opens a new one
                    btn.button.onClick.AddListener(() => { SelectRegion(_loadedRegions[btn.indexValue]); });
                }
            }
        }
        
    }

    public void AdjustRegionLayout(SelectorButton btn, int i)
    {
       float height; 
       Vector2 space = listTransformTab.GetComponent<GridLayoutGroup>().spacing; 
       float f = btn.children.position.y;
       float h = btn.children.position.x; 
       float posx = listTransformChildren.position.x; 
       height = btn.children.GetComponent<RectTransform>().rect.height; 
       Vector2 newSpace = new Vector2(150, height); 
       Debug.Log(height); 
       listTransformTab.GetComponent<GridLayoutGroup>().spacing = newSpace; 
        // if (_loadedOrgans.Count > 20)
        // {
        //     space = new Vector2(150, 500);  
        //     Debug.Log("selected 1"); 
        //    // listTransformTab.RowSpacing()
        //     //listTransformTab.GetComponent<LayoutElement>().preferredHeight = 1500; 
        // } else if ((_loadedOrgans.Count < 20) && (_loadedOrgans.Count > 10))
        // {
        //     listTransformTab.GetComponent<GridLayoutGroup>().spacing = new Vector2(150, 250);  
        //     Debug.Log("selected 2"); 
            
        // }
        //  else if (_loadedOrgans.Count == 10)
        // {
        //     space = new Vector2(150, 180);  
        //     Debug.Log("selected 3"); 
        // } else if (_loadedOrgans.Count < 10)
        // {
        //     space = new Vector2(150, 80);  
        //     Debug.Log("selected 3"); 
        // } 

        // EDIT this to the last items in the list
        // if ((_loadedRegions[i] == _loadedRegions[6]) || (_loadedRegions[i] == _loadedRegions[7]))
        // {
        //     space = new Vector2(150, 0);
        // }

        if (i >= 0 && i < 3)
        {
            for (int k = 0; k < 3; k++)
            {
                if (_loadedRegions[i] == _loadedRegions[k])
                {
                    btn.children = listTransformChildren;
                }
            }
        } 
    }

    public void ResetRegionLayout()
    {
        listTransformTab.GetComponent<GridLayoutGroup>().spacing = new Vector2(150, 0); 
        Debug.Log("reset"); 
    }
    private void ShowCoursesPage()
    {
        Clear();

        SidePanelPreviewOff();
        byLab = true;
        ShowAllCourses(listTransformAllCourses);
        homeInfo.SetActive(true);
        courseInfoContent.SetActive(false);
        RecentCourse.SetActive(false);
        welcomePanel.SetActive(false);
        labInfoContent.SetActive(false);
        secondLabel.text = "> Courses";
        thirdLabel.text = "";
        fourthLabel.text = "";
        fifthLabel.text = "";
        homeLabel.color = Color.black;
        atlasLabel.color = Color.black;
        helpLabel.color = Color.black;
        courseLabel.color = hiveBlue;
        page = CurrPage.COURSE;
        
        if (secondLabel.text == "> Courses")
        {
            second.onClick.AddListener(ShowCoursesPage);
        }
        homeScrollRect.verticalNormalizedPosition = 1.5f;
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
        byLab = true;
        Clear();
        homeScrollRect.verticalNormalizedPosition = 1.5f;
        foreach (var course in _loadedCourses.Take(num * 2))
        {
            CourseDisplayOptions courseOption = Instantiate(coursePrefab, listTransformCourses);
            courseOption.Populate(course, this, selectorMenu);
           
        };
        SidePanelPreviewOff();
        homeInfo.SetActive(true);
        courseInfoContent.SetActive(false);
        RecentCourse.SetActive(true);
        welcomePanel.SetActive(true);
        labInfoContent.SetActive(false);
        homeLabel.color = hiveBlue;
        atlasLabel.color = Color.black;
        helpLabel.color = Color.black;
        courseLabel.color = Color.black;
        listTransformCourses.GetComponent<GridLayoutGroup>().constraintCount = num;
        secondLabel.text = "";
        thirdLabel.text = "";
        fourthLabel.text = "";
        fifthLabel.text = "";
        page = CurrPage.HOME;
        
    }

    public void ShowHelpInfo()
    {
        Clear();
        SidePanelPreviewOff();
        homeInfo.SetActive(true);
        courseInfoContent.SetActive(false);
        RecentCourse.SetActive(false);
        welcomePanel.SetActive(false);
        labInfoContent.SetActive(false);
        secondLabel.text = "> Help";
        thirdLabel.text = "";
        fourthLabel.text = "";
        fifthLabel.text = "";
        homeLabel.color = Color.black;
        atlasLabel.color = Color.black;
        helpLabel.color = hiveBlue;
        courseLabel.color = Color.black;

        page = CurrPage.HELP;
        
        if (secondLabel.text == "> Help")
        {
            second.onClick.AddListener(ShowHelpInfo);
        }
    }

    public void ShowAtlasInfo()
    {
        Clear();
        SidePanelPreviewOff();
        byLab = false; 
        ClearSelectionData();
        homeInfo.SetActive(true);
        courseInfoContent.SetActive(false);
        RecentCourse.SetActive(false);
        welcomePanel.SetActive(false);
        labInfoContent.SetActive(false);
        secondLabel.text = "> 3D Atlas";
        thirdLabel.text = "";
        fourthLabel.text = "";
        fifthLabel.text = "";
        homeLabel.color = Color.black;
        atlasLabel.color = hiveBlue;
        helpLabel.color = Color.black;
        courseLabel.color = Color.black; 

        if (secondLabel.text == "> 3D Atlas")
        {
            second.onClick.AddListener(ShowAtlasInfo);
        } 
    }

    private void ShowAllLabs()
    {
        Clear();
        sidePanelScrollRect.verticalNormalizedPosition = 1.5f; 
        _loadedLabs.ForEach((lab) => {
            LabDisplayOptions labOption = Instantiate(labPrefab, listTransformLabs);
            labOption.Populate(lab, this, selectorMenu);
        });
    }

    private void ShowAllCourses(Transform listTransform)
    {
        Clear();
        sidePanelScrollRect.verticalNormalizedPosition = 1.5f; 
        foreach (var course in _loadedCourses)
        {
            CourseDisplayOptions courseOption = Instantiate(coursePrefab, listTransform);
            courseOption.Populate(course, this, selectorMenu);
          
        };
    }

    private void ShowAllLabsText()
    {
        ClearLabText();
        if (_loadedLabs.Count() == 0)
        {
            noContentText.gameObject.SetActive(true);
            noContentText.text = "No labs for this course";
        }
        else 
        {
            _loadedLabs.ForEach((lab) =>
            {
                LabDisplayOptions labOption = Instantiate(labTextPrefab, listTransformLabText);
                labOption.Populate(lab, this, selectorMenu);
                noContentText.gameObject.SetActive(false); 
            });
        }
        
    }

    private void SidePanelPreview(string id, string imgUrl)
    {
        Clear(); 
        sidePanelScrollRect.verticalNormalizedPosition = 1.5f;
        specimenRenderedImg.gameObject.SetActive(true);
        previewText.gameObject.SetActive(true);
        specimenText.gameObject.SetActive(true); 
        specimenText.text = id; 
        previewBtn.gameObject.SetActive(true);
        StartCoroutine(LoadSpecimenImg(imgUrl)); //preview specimen image 
        previewBtn.onClick.AddListener(() => selectorMenu.SelectSpecimen(id)); // Connects to selector menu to get the specimen ready 
    }

    public void SidePanelPreviewOff()
    {
        specimenRenderedImg.gameObject.SetActive(false);
        previewImage.gameObject.SetActive(false);
        specimenText.gameObject.SetActive(false); 
        previewText.gameObject.SetActive(false);
        previewBtn.gameObject.SetActive(false);
    }
   
   public void SpecimenLoadingPopUpOff()
   {
       SpecimenLoadingPopUpScreen.SetActive(false); 
   }


   public void LastEntry()
   {
       if (secondLabel.text == "> 3D Atlas")
       {
           ShowAtlasInfo(); 
       }
       else if (fourthLabel.text == $"> {labName}")
       {
           ShowLabSpecDetails(); 
       }
       else if (thirdLabel.text == $"> {courseName}")
       {
           RenderCourseInfo(courseName); 
           ShowSpecimenDetails(); 
       }

   }


    

}