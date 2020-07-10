using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Controller;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
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
    public OrbitCamera orbitCam;
    public CompareMenu compareMenu;
    public AnnotationDisplay annotationDisplay;
    public GameObject leftPanel;
    public GameObject specimenLabel;
    public SpecimenCart cart;
    public GameObject trayObj;

    public PostProcessVolume volume;
    DepthOfField depthOfField;
    public FocusDistanceFinder focusDistanceFinder;

    [Header("Focus Information")]
    public GameObject currentSelectedObject;
    public SpecimenData currentSelectedData;
    private bool _focusOn;
    private UITwoStateIndicator _focusIndicator;

    [Header("Specimen Rotation")]
    private GameObject _rotatingSpecimen;
    private float _xRot;
    private float _yRot;

    public void Activate()
    {
        if (stateController.CurrentSpecimenObject == null)
        {
            Debug.LogWarning("Entering analysis view with no subject! Something has gone wrong.");
        }

        currentSelectedObject = stateController.CurrentSpecimenObject;
        currentSelectedData = stateController.CurrentSpecimenData;
        leftPanel.gameObject.SetActive(true);
        controlAssistant.gameObject.SetActive(controlAssistToggle.on);
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
        depthOfField.active = true;
        focusDistanceFinder.enabled = true;
    }

    public void Deactivate()
    {
        _rotatingSpecimen = null;
        proportionScript.ResetProportionIndicator(); // Hide selected specimen on proportion
        uiObject.SetActive(false);
        mainCamera.GetComponent<OrbitCamera>().enabled = false;
        mainCamera.GetComponent<OrbitCamera>().target = null;
        // mainCamera.cullingMask = -1;
        trayObj.SetActive(true);
        depthOfField.active = false;
        cart.SetTrayVisibility(false);
    }


    public void Start()
    {

        orbitCam = mainCamera.GetComponent<OrbitCamera>();

        volume.profile.TryGetSettings(out depthOfField);
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
        HandleSpecimenRotation();
        HandleCamSelect();
    }

    private void HandleCamSelect() {

        if (Input.GetMouseButtonDown(0)) {
            RaycastHit hit;
            int layerMask = 9 << 9;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100.0f, layerMask)) {
                orbitCam.target = hit.transform;
                currentSelectedObject = hit.transform.gameObject;
                if (currentSelectedObject == stateController.CurrentSpecimenObject)
                {
                    currentSelectedData = stateController.CurrentSpecimenData;
                } else if (currentSelectedObject == stateController.CompareSpecimenObject)
                {
                    currentSelectedData = stateController.CompareSpecimenData;
                }
                else
                {
                    currentSelectedObject = null;
                    currentSelectedData = null;
                }

                annotationDisplay.SetFocus(currentSelectedObject, currentSelectedData);
            }
        }
    }

    private void HandleSpecimenRotation()
    {
        if (Input.GetMouseButtonDown(1)) {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 200f, ~LayerMask.NameToLayer("Specimen"))) {
                _rotatingSpecimen = hit.transform.gameObject;
            }
        }

        if (Input.GetMouseButtonUp(1)) {
            _rotatingSpecimen = null;
        }

        if (_rotatingSpecimen != null) {
            _xRot += -Input.GetAxis("Mouse X") * 5f;
            _yRot += Input.GetAxis("Mouse Y") * 5f;
            _rotatingSpecimen.transform.rotation =
                Quaternion.AngleAxis(_xRot, transform.up) * Quaternion.AngleAxis(_yRot, transform.right);

        }
    }


    // RESET BUTTON METHOD

    void ResetCameraPosition() {
        print("Reset");

        // TODO

    }

    // ANNOTATIONS

    void ToggleAnnotations(bool on) {
        annotationDisplay.gameObject.SetActive(on);
        if (on)
        {
            annotationDisplay.SetFocus(currentSelectedObject, currentSelectedData);
        }
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
