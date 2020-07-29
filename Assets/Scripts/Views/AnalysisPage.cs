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
    public Button controlButtonUp;
    public Button controlButtonDown;
    public Button controlButtonLeft;
    public Button controlButtonRight;
    public Button zoomIn;
    public Button zoomOut;

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
    public TrayPage trayPageScript;

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

    public void Activate()
    {
        if (stateController.CurrentSpecimenObject == null)
        {
            Debug.LogWarning("Entering analysis view with no subject! Something has gone wrong.");
        }

        if (depthOfField == null)
        {
            volume.profile.TryGetSettings(out depthOfField);
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

        Button up = controlButtonUp.GetComponent<Button>();
        Button down = controlButtonDown.GetComponent<Button>();
        Button left = controlButtonLeft.GetComponent<Button>();
        Button right = controlButtonRight.GetComponent<Button>();
        Button zoomInside = zoomIn.GetComponent<Button>();
        Button zoomOutside = zoomOut.GetComponent<Button>();
     


        up.onClick.AddListener(MoveUp);
        down.onClick.AddListener(MoveDown);
        left.onClick.AddListener(MoveLeft);
        right.onClick.AddListener(MoveRight);
        zoomInside.onClick.AddListener(ZoomIn);
        zoomOutside.onClick.AddListener(ZoomOut);
        
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
            RaycastHit hit;
            int layerMask = 9 << 9;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100.0f, layerMask)) {
                //orbitCam.target = hit.transform;
                if (hit.transform.gameObject == currentSelectedObject || hit.transform.parent.gameObject == currentSelectedObject) return; // Necessary to escape or we'll cut off obscured button actions (e.g. annotations)

                currentSelectedObject = hit.transform.parent.gameObject;
                if (currentSelectedObject == stateController.CurrentSpecimenObject)
                {
                    ChangeFocus(stateController.CurrentSpecimenObject, stateController.CurrentSpecimenData);
                    /*currentSelectedData = stateController.CurrentSpecimenData;
                    targetSpecimenLabel.text = currentSelectedData.name;*/
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

    public void ChangeFocus(GameObject focusObject, SpecimenData focusData)
    {
        currentSelectedData = focusData;
        orbitCam.target = focusObject.transform;
        targetSpecimenLabel.text = currentSelectedData.name;

    }


    // RESET BUTTON METHOD

    void ResetCameraPosition() {
        currentSelectedObject.transform.rotation = Quaternion.Euler(specimenRotation);
        _xRot = 0;
        _yRot = 0;


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
     * Opens in-analysis compare window
     */
    void ToggleCompare()
    {
        /*bool on = !compareMenu.gameObject.activeSelf;
        compareMenu.gameObject.SetActive(on);
        leftPanel.gameObject.SetActive(!on);
        */
        stateController.mode = ViewMode.TRAY;
        trayPageScript.SelectCompare(stateController.CurrentSpecimenData.organ);
    }

    /**
     * Hides UI (focus mode)
     */
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

    void MoveUp()
    {
        orbitCam.yVelocity += 1f * orbitCam.rotationSensitivity;
        orbitCam.yRotationAxis += orbitCam.yVelocity;
        orbitCam.yRotationAxis = orbitCam.ClampAngleBetweenMinAndMax(orbitCam.yRotationAxis, orbitCam.rotationLimit.x, orbitCam.rotationLimit.y);
    }

    void MoveDown()
    {
        orbitCam.yVelocity -= 1f * orbitCam.rotationSensitivity;
        orbitCam.yRotationAxis += orbitCam.yVelocity;
        orbitCam.yRotationAxis = orbitCam.ClampAngleBetweenMinAndMax(orbitCam.yRotationAxis, orbitCam.rotationLimit.x, orbitCam.rotationLimit.y);

    }

    void MoveLeft()
    {
        orbitCam.xVelocity += 6f * orbitCam.rotationSensitivity;
        orbitCam.yRotationAxis += orbitCam.yVelocity;

    }

    void MoveRight()
    {
        orbitCam.xVelocity -= 6f * orbitCam.rotationSensitivity;
        orbitCam.yRotationAxis += orbitCam.yVelocity;

    }

    void ZoomIn()
    {
        print("Zoom In");

    }

    void ZoomOut()
    {
        print("Zoom Out");

    }
}
