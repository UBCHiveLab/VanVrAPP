using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Controller;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnalysisPage : MonoBehaviour, IPage
{
    [Header("Control Assistant")]
    public ControlAssist controlAssistant;

    [Header("Reset Camera position")]

    [Header("Toggles")]
    public UIToggle annotationToggle;
    public UIToggle controlAssistToggle;
    public UIToggle proportionToggle;

    [Header("Buttons")]
    public Button focusModeButton;
    public Button compareButton;
    public Button resetButton;

    [Header("Proportion Indicator")]
    public ProportionIndicator proportionScript;
    public GameObject proportionIndicator;

    [Header("Other")]
    public TextMeshProUGUI targetSpecimenLabel;
    public GameObject uiObject;
    public StateController stateController;
    public Camera mainCamera;
    public CompareMenu compareMenu;
    public AnnotationDisplay annotationDisplay;
    public GameObject leftPanel;
    public GameObject specimenLabel;
    public SpecimenCart cart;
    public GameObject trayObj;

    private bool _focusOn;
    private UITwoStateIndicator _focusIndicator;

    public void Activate()
    {
        if (stateController.CurrentSpecimenObject == null)
        {
            Debug.LogWarning("Entering analysis view with no subject! Something has gone wrong.");
        }

        controlAssistant.gameObject.SetActive(false);
        compareMenu.gameObject.SetActive(false);
        ToggleAnnotations(annotationToggle.on);
        proportionIndicator.SetActive(proportionToggle.on); 
        uiObject.SetActive(true);
        mainCamera.GetComponent<Animator>().enabled = false;
        mainCamera.GetComponent<OrbitCamera>().enabled = true;
        mainCamera.GetComponent<OrbitCamera>().target = stateController.CurrentSpecimenObject.transform;
        //mainCamera.cullingMask = 9 << 9;
        targetSpecimenLabel.text = stateController.CurrentSpecimenData.name;
        trayObj.SetActive(false);
        cart.SetTrayVisibility(true);
        //annotationDisplay.Activate();
    }

    public void Deactivate()
    {
        proportionScript.ResetProportionIndicator(); // Hide selected specimen on proportion
        uiObject.SetActive(false);
        mainCamera.GetComponent<OrbitCamera>().enabled = false;
        mainCamera.GetComponent<OrbitCamera>().target = null;
        // mainCamera.cullingMask = -1;
        trayObj.SetActive(true);
        cart.SetTrayVisibility(false);

    }


    public void Start() {
        
       
        // Control Assistant Buttons
        controlAssistToggle.Bind((on) => controlAssistant.gameObject.SetActive(!controlAssistant.gameObject.activeSelf));


        // Annotation Button
        annotationToggle.Bind(ToggleAnnotations);

        // Reset Specimen Button
        resetButton.onClick.AddListener(ResetCameraPosition);

        // Compare toggle
        compareButton.onClick.AddListener(ToggleCompare);

        // Proportion toggle
        proportionToggle.Bind(ToggleProportionIndicator);

        focusModeButton.onClick.AddListener(() => ToggleFocus());
        _focusIndicator = focusModeButton.GetComponent<UITwoStateIndicator>();
    }

    public void Update()
    {

        if (stateController.mode != ViewMode.ANALYSIS) return;

    }


    // RESET BUTTON METHOD

    void ResetCameraPosition() {
        print("Reset");

        // TODO

    }

    // ANNOTATIONS

    void ToggleAnnotations(bool on) {
        annotationDisplay.gameObject.SetActive(on);
    }

    void ToggleProportionIndicator(bool on)
    {
        proportionIndicator.gameObject.SetActive(on);
    }

    void ToggleCompare()
    {
        bool on = !compareMenu.gameObject.activeSelf;
        compareMenu.gameObject.SetActive(on);
        leftPanel.gameObject.SetActive(!on);
    }


    void ToggleFocus()
    {
        // TODO: animate these
        _focusOn = !_focusOn;

        annotationDisplay.gameObject.SetActive(!_focusOn && annotationToggle.on);
        compareMenu.gameObject.SetActive(false); // Always hide compare menu
        leftPanel.gameObject.SetActive(!_focusOn);
        specimenLabel.gameObject.SetActive(!_focusOn);
        _focusIndicator.UpdateState(_focusOn);
    }
}
