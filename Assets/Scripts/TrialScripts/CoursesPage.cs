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
    public ReadInput input; 
    public MainCameraEvents camera; 

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
    public Animator anim; 
     
    public TextMeshProUGUI noContentText;

    public GameObject labAtlasToggle;
    public Button atlasButtonMain;
    public Button labButton;
     public TextMeshProUGUI atlasLabelMain;
    public TextMeshProUGUI labLabel;
    public TextMeshProUGUI titleOnShelf;

    public TextMeshProUGUI atlasLabel;
    public TextMeshProUGUI courseLabel;
    public TextMeshProUGUI homeLabel;
    public TextMeshProUGUI helpLabel; 
    public GameObject RecentCourse;
    public GameObject loadingIndicator;
    
    public GameObject homeInfo;
    public GameObject inputField; 
    

    [Header("HomeContentRender")]
    public GameObject welcomePanel;
    public GameObject expandedPanel;
    public GameObject defaultPanel;
    public ScrollRect homeScrollRect;
    public TextMeshProUGUI message; 

    [Header("CourseContentRender")]
     public GameObject courseInfoContent;
    public TextMeshProUGUI courseTitle;
    public GameObject courseInfoContentText;
    public GameObject labInfoContentText;
    public ScrollRect courseScrollRect;
    public Transform courseContent; 
    public TextMeshProUGUI labSpecName;
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
    public GameObject sidePanel;
    public Button expandPanelBtn;
    public Sprite expand;
    public Sprite collapse;
    public GameObject SpecimenLoadingPopUpScreen; 
    public Button previewBtn; 
    public Image previewImage; 
    public TextMeshProUGUI specimenText; 
    public TextMeshProUGUI previewText; 
    



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
        
        UpdateUI();
        
        homeScrollRect.verticalNormalizedPosition = 1.5f;
        // noContentText.gameObject.SetActive(false);
        homeLabel.color = Color.blue; 
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
              //  Debug.Log("lab data is here");
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
      //  CheckStringInput(input.ReadStringInput(message.text));
        Layout(mode, showNoContentText);
    }
// Figure out a way to access the script read string input from the other class - look into this
    // public void CheckStringInput(string message)
    // {
    //     // message = inputField.GetComponent<Text>().text;
    //     // Debug.Log(message); 
    //     // if (!string.IsNullOrEmpty(search))
    //     // {
    //     for (int i = 0; i < _loadedCourses.Count; i++ )
    //     {
    //         if (CheckString().Contains(_loadedCourses[i].courseId))
    //         {
    //             CourseDisplayOptions courseOption = Instantiate(coursePrefab, listTransformAllCourses);
    //               //  courseOption.Populate(course, this, selectorMenu);
    //             Debug.Log(_loadedCourses[i].courseId);
    //             Debug.Log("loaded search courses");    
    //         }
    //     }
    //     //}
       
    // }
    

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
       // var newCourseName = "This Lab is about " + courseId;
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
        coursePageInfoLabel.color = Color.cyan;
        coursePageLabLabel.color = Color.white;
        coursePageSpecLabel.color = Color.white;
        secondLabel.text = "> Courses";
        thirdLabel.text = $"> {courseName}";
        fourthLabel.text = "";
        fifthLabel.text = "";
        
        ShowAllCourses(listTransformSideCourses);
        _loadedLabs = store.GetLabDataForCourse(courseId);
     //   Debug.Log(_loadedLabs.Count());

        ShowAllLabsText();
      //  courseLayoutElement.preferredHeight 
     //   Debug.Log("show all labs text");
        // if (_loadedLabs != null)
        // {
        //     ShowAllLabsText();
        //     Debug.Log("show all labs text");
        // }

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
     //   ClearSpec();
    
        ClearLabSpec();
        mode = ListMode.COURSE_SPECIMENS;
        

        // Clear();
        courseInfoContentText.SetActive(false);
        courseScrollRect.verticalNormalizedPosition = 1.5f;
        coursePageInfoLabel.color = Color.white;
        coursePageLabLabel.color = Color.white;
        coursePageSpecLabel.color = Color.cyan;
        labSpecDisplayPrefab.gameObject.SetActive(true);
        
        List<Tuple<int, List<SpecimenData>>> courseSpecData = store.GetSpecimenData(courseId);
      //  if (courseSpecData.Count() )
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
                Debug.Log("count greater than 12"); 
            } else if ((_loadedCourseSpecimens.Count() > 8) && (_loadedCourseSpecimens.Count() <= 12))
            {
                listTransformLabName.GetComponent<VerticalLayoutGroup>().spacing = 100; 
                options.children.GetComponent<GridLayoutGroup>().padding.top = 0; 
                Debug.Log("count greater than 8"); 
            } else if ((_loadedCourseSpecimens.Count() > 4) && (_loadedCourseSpecimens.Count() <= 8))
            {
                listTransformLabName.GetComponent<VerticalLayoutGroup>().spacing = 100; 
                options.children.GetComponent<GridLayoutGroup>().padding.top = -60; 
                Debug.Log("count greater than 4"); 
            }
            else if ((_loadedCourseSpecimens.Count() <= 4))
            {
                listTransformLabName.GetComponent<VerticalLayoutGroup>().spacing = 100; 
                options.children.GetComponent<GridLayoutGroup>().padding.top = -100;
                Debug.Log("count greater than 0");  
            }

            for (int i = 0; i < _loadedCourseSpecimens.Count; i++)
            {
                string id = _loadedCourseSpecimens[i].id;
                string imgUrl = _loadedCourseSpecimens[i].imgUrl; 
                SelectorButton btn = Instantiate(lightSelectorPrefab, options.children);
                btn.Populate(_loadedCourseSpecimens[i].name, i, null);
             //   Debug.Log(_loadedCourseSpecimens[i].name);
                btn.button.onClick.AddListener(() => SidePanelPreview(id, imgUrl));
             //   idToButton.Add(id, btn);
             //   Debug.Log("course specimen reached layout room");
       
            }
            
        }  
        
        
        
    }
    private void ShowLabSpecDetails()
    {
        ClearSpec();
        mode = ListMode.LAB_SPECIMENS;
        labInfoContentText.SetActive(false);
        
        Tuple<string, List<SpecimenData>> labData = store.GetLabData(courseId, labId);
        //  selectionTitle.text = labData.Item1;
        _loadedSpecimens = labData.Item2;
        showNoContentText = _loadedSpecimens == null || _loadedSpecimens.Count < 1;
     //   Debug.Log("specs2 are loaded here");
        labPageSpecLabel.color = Color.blue;
        labPageInfoLabel.color = Color.white;
        Layout(mode, showNoContentText);
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
      //  Debug.Log("lab data is here");
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
       var newLabName = "This Lab is about " + labName;
        RenderLabInfo(labName, labId, labImg);
        fourth.onClick.AddListener(() => RenderLabInfo(labName, labId, labImg));
        fourth.onClick.AddListener(() => {selectorMenu.RenderLabInfo(labName, newLabName, labImg); });
   
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
        labScrollRect.verticalNormalizedPosition = 1.5f;
        ClearSpec();
        trayPage.SetActionOff();
        //  labDescription.text = labDes;
        labInfoContent.SetActive(true);
        ShowAllLabs();
        fourthLabel.text = $"> {labName}";
        fifthLabel.text = "";
        //  selectionTitle.text = $"Home > Courses > {courseName} > {labName}";
        labPanelCourseBtn.onClick.AddListener(() => CourseSelected(courseName));
        labPanelCourseBtn.onClick.AddListener(() => selectorMenu.CourseSelected(courseName));
        labPanelCourseBtn.onClick.AddListener(() => selectorMenu.ClearOrganAndLabData());
        labPanelCourseBtn.onClick.AddListener(()=> {selectorMenu.labInfoContent.SetActive(false); });

        specLabShowBtn.onClick.AddListener(ShowLabSpecDetails);
        labInfoShowBtn.onClick.AddListener(() => RenderLabInfo(title, labId, urlImg));
        labInfoShowBtn.onClick.AddListener(() => SidePanelPreviewOff());
        labPageInfoLabel.color = Color.blue;
        labPageSpecLabel.color = Color.white;
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
        
        loadingIndicator.gameObject.SetActive(store.Loading());
  
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
             //   btn.button.onClick.AddListener(() => selectorMenu.UpdateSelected()); 
                    btn.button.onClick.AddListener(() => SidePanelPreview(id, imgUrl));
            //     btn.button.onClick.AddListener(() => SpecimenLoadingPopUpScreen.SetActive(true));
            //     btn.button.onClick.AddListener(() => selectorMenu.SelectSpecimen(id));     
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
                //     btn.button.onClick.AddListener(() => SpecimenLoadingPopUpScreen.SetActive(true)); 
                //   //  btn.button.onClick.AddListener(() => camera.SwitchCamera());
                //     btn.button.onClick.AddListener(() => trayPage.SetAnalyzeOn());
                //     btn.button.onClick.AddListener(() => selectorMenu.SelectSpecimen(id));     
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
                // btn.button.onClick.AddListener(() => trayPage.SetAnalyzeOn());
                // btn.button.onClick.AddListener(() => SpecimenLoadingPopUpScreen.SetActive(true));
                // btn.button.onClick.AddListener(() => selectorMenu.SelectSpecimen(id));
                // btn.button.onClick.AddListener(() => SpecimenLoadingPopUpScreen.SetActive(true));
            
                
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
       // Clear(); 
       Vector2 space = listTransformTab.GetComponent<GridLayoutGroup>().spacing; 
       float f = btn.children.position.y;
        if (_loadedOrgans.Count > 20)
        {
            space = new Vector2(150, 500);  
            Debug.Log("selected 1"); 
           // listTransformTab.RowSpacing()
            //listTransformTab.GetComponent<LayoutElement>().preferredHeight = 1500; 
        } else if ((_loadedOrgans.Count < 20) && (_loadedOrgans.Count > 10))
        {
            listTransformTab.GetComponent<GridLayoutGroup>().spacing = new Vector2(150, 250);  
            Debug.Log("selected 2"); 
            
        }
         else if (_loadedOrgans.Count == 10)
        {
            space = new Vector2(150, 180);  
            Debug.Log("selected 3"); 
        } else if (_loadedOrgans.Count < 10)
        {
            space = new Vector2(150, 80);  
            Debug.Log("selected 3"); 
        } 

        if ((_loadedRegions[i] == _loadedRegions[6]) || (_loadedRegions[i] == _loadedRegions[7]))
        {
            space = new Vector2(150, 0);
        }

        for (int k = 0; k < 3; k++)
            {
                if (_loadedRegions[i] == _loadedRegions[k])
                {
                    btn.children = listTransformChildren;
                }
            }

            for (int k = 3; k < 6; k++)
            {
                if (_loadedRegions[i] == _loadedRegions[k])
                {
               
                    f = listTransformChildren.position.y + space.y; 
                }
            }
        
        // if ((_loadedRegions[i] == _loadedRegions[0]) || (_loadedRegions[i] == _loadedRegions[1]) || (_loadedRegions[i] == _loadedRegions[2]))
        // {
        //     btn.hildren.transform.localPosition = new Vector3(-2, -1, 0); 
        // }
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
        //  selectionTitle.text = "Home > Courses";
        homeLabel.color = Color.black;
        atlasLabel.color = Color.black;
        helpLabel.color = Color.black;
        courseLabel.color = Color.blue;
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
        foreach (var course in _loadedCourses.Take(num))
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
        homeLabel.color = Color.blue;
        atlasLabel.color = Color.black;
        helpLabel.color = Color.black;
        courseLabel.color = Color.black;
      //  selectionTitle.text = "Home";
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
     //   selectionTitle.text = "Home > Help";
        homeLabel.color = Color.black;
        atlasLabel.color = Color.black;
        helpLabel.color = Color.blue;
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
        atlasLabel.color = Color.blue;
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
        
        _loadedLabs.ForEach((lab) => {
            LabDisplayOptions labOption = Instantiate(labPrefab, listTransformLabs);
            labOption.Populate(lab, this, selectorMenu);
        });
    }

    private void ShowAllCourses(Transform listTransform)
    {
        Clear();
        foreach (var course in _loadedCourses)
        {
            CourseDisplayOptions courseOption = Instantiate(coursePrefab, listTransform);
            courseOption.Populate(course, this, selectorMenu);
          
        };
    }

    private void ShowAllLabsText()
    {
        ClearLabText();
     //   Debug.Log("after clearing lab text");
        if (_loadedLabs.Count() == 0)
        {
            noContentText.gameObject.SetActive(true);
            noContentText.text = "No labs for this course";
        //    Debug.Log(noContentText.text); 
        }
        else 
        {
            _loadedLabs.ForEach((lab) =>
            {
                LabDisplayOptions labOption = Instantiate(labTextPrefab, listTransformLabText);
                labOption.Populate(lab, this, selectorMenu);
                noContentText.gameObject.SetActive(false);
             //   Debug.Log("lab option");
            });
        }
        
    }

    private void SidePanelPreview(string id, string imgUrl)
    {
        Clear(); 
        specimenRenderedImg.gameObject.SetActive(true);
        previewText.gameObject.SetActive(true);
        specimenText.gameObject.SetActive(true); 
        specimenText.text = id; 
        previewBtn.gameObject.SetActive(true);
        StartCoroutine(LoadSpecimenImg(imgUrl)); ; 
        previewBtn.onClick.AddListener(() => selectorMenu.SelectSpecimen(id));
      //  previewBtn.onClick.AddListener(() => StartCoroutine(SpecimenLoadingPopUpOn()));

        //   previewBtn.onClick.AddListener(() => selectorMenu.UpdateSelected());
        //  previewBtn.onClick.AddListener(() => trayPage.SetAnalyzeOn());
    }

    public void SidePanelPreviewOff()
    {
        specimenRenderedImg.gameObject.SetActive(false);
        previewImage.gameObject.SetActive(false);
        specimenText.gameObject.SetActive(false); 
        previewText.gameObject.SetActive(false);
        previewBtn.gameObject.SetActive(false);
    }
   

   public IEnumerator SpecimenLoadingPopUpOn()
   {
       SpecimenLoadingPopUpScreen.SetActive(true);
        yield return new WaitForSeconds(1.5f);
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