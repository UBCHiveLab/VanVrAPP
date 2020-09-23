using Assets.Scripts.State;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

/**
 * Defines logic and composes components for Analysis mode
 */
public class AnalysisPage : MonoBehaviour, IPage
{
    [Header("Control Assistant")]
    public ControlAssist controlAssistant;

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
    public PostProcessVolume volume;
    private DepthOfField depthOfField;
    public FocusDistanceFinder focusDistanceFinder;

    [Header("Focus Information")]
    public GameObject currentSelectedObject;
    public SpecimenData currentSelectedData;
    private bool _focusOn;
    private UITwoStateIndicator _focusIndicator;

    [Header("Specimen Rotation")]
    private GameObject _rotatingSpecimen;
    private Vector3 specimenRotation;
    private float _xRot;
    private float _yRot;
    private bool resetSpecimen;

    [Header("External")]
    public ErrorPanel errorPanel;
    public SpecimenCart cart;
    public TextMeshProUGUI targetSpecimenLabel;
    public GameObject uiObject;
    public StateController stateController;
    public Camera mainCamera;
    public OrbitCamera orbitCam;
    public AnnotationDisplay annotationDisplay;
    public GameObject leftPanel;
    public GameObject specimenLabel;
    public GameObject trayObj;
    public TrayPage trayPageScript;


    public void Activate()
    {
        if (stateController.CurrentSpecimenObject == null)
        {
            Debug.LogWarning("Entering analysis view with no subject! Something has gone wrong.");
            errorPanel.Populate("Entered Analysis Mode without a subject. Something went wrong!");
            stateController.mode = ViewMode.TRAY;
        }

        if (depthOfField == null)
        {
            volume.profile.TryGetSettings(out depthOfField);
        }

        // Sets up all configurations for Analysis mode

        currentSelectedObject = stateController.CurrentSpecimenObject;
        currentSelectedData = stateController.CurrentSpecimenData;
        leftPanel.gameObject.SetActive(true);
        controlAssistant.gameObject.SetActive(controlAssistToggle.on);
        ToggleAnnotations(annotationToggle.on);
        proportionIndicator.SetActive(proportionToggle.on); 
        uiObject.SetActive(true);
        mainCamera.GetComponent<Animator>().enabled = false;
        mainCamera.GetComponent<OrbitCamera>().enabled = true;
        mainCamera.GetComponent<OrbitCamera>().target = stateController.CurrentSpecimenObject.transform;
        targetSpecimenLabel.text = currentSelectedData.name;
        trayObj.SetActive(false);
        cart.SetTrayVisibility(true);
        depthOfField.active = true;
        focusDistanceFinder.enabled = true;
        specimenRotation = currentSelectedObject.transform.rotation.eulerAngles;
    }

    public void Deactivate()
    {
        if (depthOfField == null) {
            volume.profile.TryGetSettings(out depthOfField);
        }

        // Cleans up all configurations for Analysis mode

        _rotatingSpecimen = null;
        proportionScript.ResetProportionIndicator(); // Hide selected specimen on proportion
        uiObject.SetActive(false);
        mainCamera.GetComponent<OrbitCamera>().enabled = false;
        mainCamera.GetComponent<OrbitCamera>().target = null;
        trayObj.SetActive(true);
        depthOfField.active = false;
        cart.SetTrayVisibility(false);
    }


    public void Start()
    {
        orbitCam = mainCamera.GetComponent<OrbitCamera>();
        volume.profile.TryGetSettings(out depthOfField);

        // Control Assistant Toggle
        controlAssistToggle.Bind((on) => controlAssistant.gameObject.SetActive(!controlAssistant.gameObject.activeSelf));

        // Annotation Button
        annotationToggle.Bind(ToggleAnnotations);

        // Reset Specimen Button
        resetButton.onClick.AddListener(ResetCameraPosition);

        // Compare toggle
        compareButton.onClick.AddListener(ToggleCompare);

        // Proportion toggle
        proportionToggle.Bind(ToggleProportionIndicator);

        // Focus mode (with icon indicator)
        focusModeButton.onClick.AddListener(() => ToggleFocus());
        _focusIndicator = focusModeButton.GetComponent<UITwoStateIndicator>();

    }

    public void Update()
    {
        if (stateController.mode != ViewMode.ANALYSIS) return;
        HandleSpecimenRotation();
        HandleCamSelect();
    }

    /**
     * Allows us to switch focus to a different specimen by left clicking
     */
    private void HandleCamSelect() {

        if (Input.GetMouseButtonDown(0)) {
            int layerMask = 9 << 9;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, 100.0f, layerMask)) {
                if (hit.transform.gameObject == currentSelectedObject || hit.transform.parent.gameObject == currentSelectedObject) return; // Necessary to escape or we'll cut off obscured button actions (e.g. annotations)

                currentSelectedObject = hit.transform.parent.gameObject;
                if (currentSelectedObject == stateController.CurrentSpecimenObject)
                {
                    ChangeFocus(stateController.CurrentSpecimenObject, stateController.CurrentSpecimenData);
                } else if (currentSelectedObject == stateController.CompareSpecimenObject)
                {
                    ChangeFocus(stateController.CompareSpecimenObject, stateController.CompareSpecimenData);
                } else
                {
                    currentSelectedObject = null;
                    currentSelectedData = null;
                }
                annotationDisplay.SetFocus(currentSelectedObject, currentSelectedData);
            }
        }
    }


    /**
     * Allows us to rotate the specimen by right clicking and dragging
     */
    private void HandleSpecimenRotation()
    {
       
        if (Input.GetMouseButtonDown(1)) {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 200f, ~LayerMask.NameToLayer("Specimens"))) {
                _rotatingSpecimen = hit.transform.parent.gameObject;
                
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

    /**
     * Changes current camera focus
     */
    public void ChangeFocus(GameObject focusObject, SpecimenData focusData)
    {
        currentSelectedData = focusData;
        orbitCam.target = focusObject.transform;
        targetSpecimenLabel.text = currentSelectedData.name;
    }


    /**
     * Resets specimen rotation and camera prosition
     */
    void ResetCameraPosition() {
        currentSelectedObject.transform.rotation = Quaternion.Euler(specimenRotation);
        _xRot = 0;
        _yRot = 0;
        orbitCam.yRotationAxis = 0;
        orbitCam.xRotationAxis = 0;
        orbitCam.xVelocity = 0;
        orbitCam.yVelocity = 0;

        //mainCamera.transform.position = new Vector3(0.22f, 1.91f, 20.04f);
        //mainCamera.transform.rotation = Quaternion.Euler(-4.211f, 0, 0);
        //mainCamera.transform.position = orbitCam.camDefaultPosition;
        //mainCamera.transform.rotation = orbitCam.camDefaultRotation;


    }

    /**
     * Toggles visibility of annotations, annotation bar and detail view
     */
    void ToggleAnnotations(bool on) {
        annotationDisplay.gameObject.SetActive(on);
        if (on)
        {
            annotationDisplay.SetFocus(currentSelectedObject, currentSelectedData);
        }
    }

    /**
     * Toggles visibility of proportion indicator
     */
    void ToggleProportionIndicator(bool on)
    {
        proportionIndicator.gameObject.SetActive(on);
    }

    /**
     * Returns the controller to tray mode and activates compare
     */
    void ToggleCompare()
    {
        stateController.mode = ViewMode.TRAY;
        trayPageScript.SelectCompare(stateController.CurrentSpecimenData.organ);
    }

    /**
     * Hides UI (focus mode)
     */
    void ToggleFocus()
    {
        _focusOn = !_focusOn;
        annotationDisplay.gameObject.SetActive(!_focusOn && annotationToggle.on);
        leftPanel.gameObject.SetActive(!_focusOn);
        specimenLabel.gameObject.SetActive(!_focusOn);
        _focusIndicator.UpdateState(_focusOn);
    }
}
