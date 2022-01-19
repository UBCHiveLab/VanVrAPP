using Assets.Scripts.State;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using UnityEngine.Networking; 
/**
 * Defines logic and composes components for Tray mode.
 */
public class TrayPage : MonoBehaviour, IPage
{
    public StateController stateController;
    public SelectorMenu selectorMenu;
    public GameObject uiObject;
    public GameObject actionButtons;
    public Button compareSameButton;
    public Button compareDifferentButton;
    public TextMeshProUGUI compareSameLabel;
    public Button analyzeButton;
    public Button removeOnlyButton;
    public Button removePrimaryButton;
    public Button removeCompareButton;
    public HoverButton startHover;
    public UITwoStateIndicator removeCompareIndicator;
    public ProportionIndicator proportionScript;
    public bool selectingCompareSpecimen;
    public Button shelfToggle;
    public GameObject errorPopUp; 
    private bool showMenu;
    public SpecimenStore store;
    public MainCameraEvents cameraEvents; 

    public FocusDistanceFinder focusDistanceFinder;

    // TEMP: we probably want to animate camera back to tray when finished
    public bool camSet;
    private Vector3 camDefaultPosition = new Vector3(0.22f, 1.91f, 20.04f);
    private Vector3 camDefaultRotation = new Vector3(-4.211f, 0f, 0f);
    private float camDefaultFov = 60f;

    public SpecimenCart cart;
    public AnalysisPage analysisPage; 
    public Animator animation; 

    void Start()
    {
        compareDifferentButton.onClick.AddListener(() => SelectCompare(null));
        analyzeButton.gameObject.SetActive(false);
        analyzeButton.onClick.AddListener(SelectAnalysis);
        
        shelfToggle.onClick.AddListener(ToggleShelfMenu); 
        actionButtons.SetActive(false);

        removeOnlyButton.onClick.AddListener(() => RemoveSpecimen(true));
        removeCompareButton.onClick.AddListener(() => RemoveSpecimen(false));
        removePrimaryButton.onClick.AddListener(() => RemoveSpecimen(true));

    }

    public void SetOrgan(string organId)
    {
        if (organId == null)
        {
            selectorMenu.SetOrganRegion(null, null);
            return;
        }

        if (!store.organToRegion.ContainsKey(organId))
        {
            Debug.LogWarning($"No entry for {organId}");
            return;
        }

        RegionData region = store.organToRegion[organId];
        selectorMenu.SetOrganRegion(organId, region);
    }

    public void Activate() {
        focusDistanceFinder.enabled = true;
        uiObject.SetActive(true);
        //LayoutStateNoSpecimens()
        SelectLayout();
        // TEMP: use animation
        if (camSet)
        {
            selectorMenu.anim.SetBool("ShowTab", true);
            Camera.main.transform.position = camDefaultPosition;
            Camera.main.transform.rotation = Quaternion.Euler(camDefaultRotation);
            Camera.main.fieldOfView = camDefaultFov;
        }

        // If no current specimen...
        if (ReferenceEquals(stateController.CurrentSpecimenData, null))
        {
            // ... hide the action buttons, turn off compare if relevant.
            actionButtons.SetActive(false);
            if (selectingCompareSpecimen) {
                CompareOff();
            }
        } else {
            // Else put compare mode on
            if (stateController.CompareSpecimenData != null)
            {
                CompareOn();
            }
            else
            {
                CompareOff();
            }
        }

    }

    public void Deactivate() {
        uiObject.SetActive(false);
        shelfToggle.gameObject.SetActive(false);
        showMenu = false;
    }

    public void SpecimenSelected(SpecimenData data)
    {
      //  SetAnalyzeOn();
        
        analyzeButton.interactable = false;

        if (selectingCompareSpecimen)
        {
            StartCoroutine(stateController.AddCompareSpecimen(data, OnAddCompareSpecimen));

        } else
        {
            // cameraEvents.SwitchCamera();
            // SetAnalyzeOn(); 
            StartCoroutine(stateController.AddPrimarySpecimen(data, OnAddPrimarySpecimen));
            
        }
     //  StartCoroutine(SpecimenLoading());
        //  actionButtons.SetActive(true);

    }

    private void OnAddPrimarySpecimen(GameObject obj)
    {
        cart.AddSpecimenPrimary(obj);
     //   StartCoroutine(SpecimenLoading()); 
        SelectAnalysis(); 
        LayoutStatePrimaryOnly();
    }
    
    private IEnumerator SpecimenLoading()
    {
        store.LoadingPopUp();
        Debug.Log("loading pop up"); 
        yield return new WaitForSeconds(1.5f); 
    }

    private void OnAddCompareSpecimen(GameObject obj) {
        if (stateController.CurrentSpecimenObject == null)
        {
            cart.AddSpecimenPrimary(obj);
        }
        else
        {
            cart.AddSpecimenCompare(obj);
            removeCompareIndicator.UpdateState(true);
        }

        analyzeButton.interactable = true;
    }

    public void HaveArrivedAtTray()
    {
        stateController.mode = ViewMode.TRAY;
        selectorMenu.anim.SetBool("ShowTab", true);
    }

    public void SelectAnalysis()
    {
        // TODO: THIS is the source of the camera bug. change later when we have final animations
        /*camDefaultPosition = Camera.main.transform.position;
        camDefaultRotation = Camera.main.transform.rotation.eulerAngles;
        camDefaultFov = Camera.main.fieldOfView;*/
        // camSet = true;

        if (stateController.CompareSpecimenObject == null)
        {
            CompareOff();
        }
        proportionScript.HighlightProportionIndicator(); // Show proportion indicator
        stateController.mode = ViewMode.ANALYSIS;
    //    cameraEvents.SwitchToAnalysis();
     //   analysisPage.ResetCameraPosition();
    }

    public void SelectCompare(string organ)
    {
        selectorMenu.anim.SetBool("ShowMenu", true);

        if (!selectingCompareSpecimen)
        {
            CompareOn();
            LayoutStateCompareSpecimens();
        }

        SetOrgan(organ);
    }


    public void RemoveEitherActiveSpecimen(string specId)
    {
        bool primary = stateController.currentSpecimenId == specId;
        if (!primary)
        {
            if (stateController.CompareSpecimenData == null || stateController.CompareSpecimenData.id != specId)
            {
                Debug.LogWarning("Trying to remove a non-selected specimen");
                return;
            }
        }

        RemoveSpecimen(primary);
    }

    public void RemoveSpecimen(bool primary)
    {
        if (primary)
        {
            stateController.RemovePrimarySpecimen();
            if (stateController.CurrentSpecimenData != null)
            {
                cart.AddSpecimenPrimary(stateController.CurrentSpecimenObject);
                if (selectingCompareSpecimen)
                {
                    LayoutStateCompareSpecimens();
                } else
                {
                    LayoutStatePrimaryOnly();
                }
            }
            else
            {
                selectingCompareSpecimen = false;
                LayoutStateNoSpecimens();
            }
        }
        else
        {
            if (stateController.CompareSpecimenData != null)
            {
                stateController.RemoveCompareSpecimen();
            } else
            {
                CompareOff();
                LayoutStatePrimaryOnly();
            }

            removeCompareIndicator.UpdateState(false);

        }

        selectorMenu.UpdateSelected();
    }



    public void ToggleShelfMenu()
    {
        showMenu = !showMenu;
        selectorMenu.anim.SetBool("ShowMenu", showMenu);
    }

    private void CompareOff()
    {
        selectingCompareSpecimen = false;
        stateController.RemoveCompareSpecimen();
        selectorMenu.EndCompare();
        cart.RemoveTray2();
        removeCompareIndicator.UpdateState(false);
    }

    private void CompareOn()
    {
        selectingCompareSpecimen = true;
        selectorMenu.SelectCompare();
        cart.SpawnTray2();
        removeCompareIndicator.UpdateState(true);
    }

    private void SelectLayout()
    {
        if (stateController.CurrentSpecimenData == null)
        {
            LayoutStateNoSpecimens();
        } else if (stateController.CompareSpecimenData == null && !selectingCompareSpecimen)
        {
            LayoutStatePrimaryOnly();
        }
        else
        {
            LayoutStateCompareSpecimens();
        }
    }

    private void LayoutStatePrimaryOnly()
    {
      //  compareSameButton.gameObject.SetActive(true);
        //compareDifferentButton.gameObject.SetActive(true);
       // removeOnlyButton.gameObject.SetActive(true);
        removeCompareButton.gameObject.SetActive(false);
        removePrimaryButton.gameObject.SetActive(false);
        analyzeButton.interactable = true;
        compareSameButton.interactable = true;
        compareSameButton.onClick.RemoveAllListeners();
        compareSameButton.onClick.AddListener(() =>
        {
            SelectCompare(stateController.CurrentSpecimenData.organ);
        });
        //compareSameLabel.text = $"COMPARE \n {stateController.CurrentSpecimenData.organ.ToUpper()}";
        compareDifferentButton.interactable = true;
        removeCompareIndicator.UpdateState(true);
        startHover.Enable();
        

    }

    private void LayoutStateNoSpecimens()
    {
        removeOnlyButton.gameObject.SetActive(false);
        removeCompareButton.gameObject.SetActive(false);
        removePrimaryButton.gameObject.SetActive(false);
        analyzeButton.interactable = false;
        compareSameButton.interactable = false;
        cart.RemoveTray2();
        compareDifferentButton.interactable = false;
        compareSameButton.gameObject.SetActive(false);
        compareDifferentButton.gameObject.SetActive(false);
        startHover.Disable();
    }

    private void LayoutStateCompareSpecimens()
    {
      //  compareSameButton.gameObject.SetActive(true);
        //compareDifferentButton.gameObject.SetActive(true);
        removeOnlyButton.gameObject.SetActive(false);
        removeCompareButton.gameObject.SetActive(true);
        removePrimaryButton.gameObject.SetActive(true);
        analyzeButton.interactable = true;
        compareSameButton.interactable = true;
        //compareSameLabel.text = $"COMPARE \n {stateController.CurrentSpecimenData.organ.ToUpper()}";
        //compareDifferentButton.interactable = true;
        removeCompareIndicator.UpdateState(false);
        startHover.Enable();
    }

    public void SetAnalyzeOn()
    {
        // analyzeButton.gameObject.SetActive(true);
        // actionButtons.gameObject.SetActive(true); 
        // removeOnlyButton.gameObject.SetActive(true); 
        // analyzeButton.onClick.AddListener(SelectAnalysis); 
        
      //  animation.StartCoroutine(CheckAnimationCompleted("SkylerProgress")); 
        while (animation.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
            SelectAnalysis(); 
      //  analysisPage.ResetCameraPosition(); 
      //  actionButtons.gameObject.SetActive(true); 
    }
    public void SetAnalyzeOff()
    {
        analyzeButton.gameObject.SetActive(false);
    }

    public void SetActionOff()
    {
        analyzeButton.gameObject.SetActive(false); 
        actionButtons.gameObject.SetActive(false); 
        removeOnlyButton.gameObject.SetActive(false); 
    }


}
