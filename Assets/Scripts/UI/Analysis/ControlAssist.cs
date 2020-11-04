using UnityEngine;
using UnityEngine.UI;
using TMPro;
/**
 * Provides a clickable interface for the orbit camera.
 */

public class ControlAssist : MonoBehaviour
{
    [Header("Buttons")]
    public Button controlButtonUp;
    public Button controlButtonDown;
    public Button controlButtonLeft;
    public Button controlButtonRight;
    public Button controlButtonZoomIn;
    public Button controlButtonZoomOut;
    public Button controlButtonXClockWise;
    public Button controlButtonXCounterClockWise;
    public Button controlButtonYClockWise;
    public Button controlButtonYCounterClockWise;
    public Button controlButtonMore;
    public Button controlButtonBack;
    public Button controlButtonResetCam;
    public Button controlButtonResetRotation;
    public Button controlButtonControlAssist;

    [Header("Comparison Mode Buttons")]
    public Button comparedControlButtonZoomIn;
    public Button comparedControlButtonZoomOut;
    public Button comparedControlButtonXClockWise;
    public Button comparedControlButtonXCounterClockWise;
    public Button comparedControlButtonYClockWise;
    public Button comparedControlButtonYCounterClockWise;
    public Button comparedResetRotation;


    [Header("Control Assist Panel")]

    public GameObject controlAssistFolded;
    public GameObject controlAssistExtended;
    public bool isControlAssistOn = true;

    [Header("Slider")]

    public float mouseSpeed;
    public TextMeshProUGUI sliderNum;

    [Header("External")]
    private OrbitCamera orbitCam;
    private AnalysisPage analysisPage;
    private ComparisonMode comparisonMode;
    private GameObject specimenHolder;
    public SpecimenCart cart;
    

    void Start()
    {
        controlButtonXClockWise.onClick.AddListener(ModelRoatateXClockWise);
        controlButtonXCounterClockWise.onClick.AddListener(ModelRoatateXCounterClockWise);
        controlButtonYClockWise.onClick.AddListener(ModelRoatateYClockWise);
        controlButtonYCounterClockWise.onClick.AddListener(ModelRoatateYCounterClockWise);
        controlButtonUp.onClick.AddListener(MoveUp);
        controlButtonDown.onClick.AddListener(MoveDown);
        controlButtonLeft.onClick.AddListener(MoveLeft);
        controlButtonRight.onClick.AddListener(MoveRight);
        controlButtonZoomIn.onClick.AddListener(ZoomIn);
        controlButtonZoomOut.onClick.AddListener(ZoomOut);
        controlButtonMore.onClick.AddListener(ControlAssistMore);
        controlButtonBack.onClick.AddListener(ControlAssistBack);
        controlButtonResetCam.onClick.AddListener(ResetCamera);
        controlButtonResetRotation.onClick.AddListener(ResetRotation);
        controlButtonControlAssist.onClick.AddListener(ControlAssistState);
        comparedControlButtonZoomIn.onClick.AddListener(ComparedZoomIn);
        comparedControlButtonZoomOut.onClick.AddListener(ComparedZoomOut);
        comparedControlButtonXClockWise.onClick.AddListener(ComparedModelRoatateXClockWise);
        comparedControlButtonXCounterClockWise.onClick.AddListener(ComparedModelRoatateXCounterClockWise);
        comparedControlButtonYClockWise.onClick.AddListener(ComparedModelRoatateYClockWise);
        comparedControlButtonYCounterClockWise.onClick.AddListener(ComparedModelRoatateYCounterClockWise);

        comparedResetRotation.onClick.AddListener(ComparedResetRotation);

        mouseSpeed = 1.0f;

        comparisonMode = GameObject.Find("UIManager").GetComponent<ComparisonMode>();

    }

    void Update()
    {
        KeyBoardControl();
        
        //When the comparison mode is on, force close the extended control assist
        if(comparisonMode.isCompared == true)
        {
            controlAssistFolded.SetActive(isControlAssistOn);
            controlAssistExtended.SetActive(!isControlAssistOn);
        }

        
    }

    void OnEnable()
    {
        orbitCam = Camera.main.GetComponent<OrbitCamera>();
        orbitCam.controlAssistActive = true;
        analysisPage = GameObject.Find("UIManager").GetComponent<AnalysisPage>();
    }

    void OnDisable()
    {
        orbitCam.controlAssistActive = false;
    }


    void MoveUp() {
        orbitCam.yVelocity += 1f * orbitCam.rotationSensitivity * mouseSpeed;
        orbitCam.yRotationAxis += orbitCam.yVelocity;
        orbitCam.yRotationAxis = orbitCam.ClampAngleBetweenMinAndMax(orbitCam.yRotationAxis, orbitCam.rotationLimit.x, orbitCam.rotationLimit.y);
    }

    void MoveDown() {
        orbitCam.yVelocity -= 1f * orbitCam.rotationSensitivity * mouseSpeed;
        orbitCam.yRotationAxis += orbitCam.yVelocity;
        orbitCam.yRotationAxis = orbitCam.ClampAngleBetweenMinAndMax(orbitCam.yRotationAxis, orbitCam.rotationLimit.x, orbitCam.rotationLimit.y);

    }

    void MoveLeft() {
        orbitCam.xVelocity += 6f * orbitCam.rotationSensitivity * mouseSpeed;
        orbitCam.yRotationAxis += orbitCam.yVelocity;

    }

    void MoveRight() {
        orbitCam.xVelocity -= 6f * orbitCam.rotationSensitivity * mouseSpeed;
        orbitCam.yRotationAxis += orbitCam.yVelocity;

    }

    void ZoomIn() {
        //check whether it's comparison mode
        if (comparisonMode.isCompared == true)
        {
            //Debug.Log(comparisonMode.isCompared);
            comparisonMode.Tray1ZoomInComparison(0.1f);
        }
        else
        {
            orbitCam.DoZoom(1f, Time.deltaTime);
        }
        

    }

    void ZoomOut() {
        //check whether it's comparison mode
        if (comparisonMode.isCompared == true)
        {
            comparisonMode.Tray1ZoomOutComparison(0.1f);
        }
        else
        {
            orbitCam.DoZoom(-1f, Time.deltaTime);
        }      

    }

    void ModelRoatateXClockWise() {
        
        specimenHolder.transform.rotation = Quaternion.AngleAxis(3 * mouseSpeed, Vector3.down) * specimenHolder.transform.rotation;
        //Debug.Log(mouseSpeed);
    }

    void ModelRoatateXCounterClockWise() {
        
        specimenHolder.transform.rotation = Quaternion.AngleAxis(3 * mouseSpeed, Vector3.up) * specimenHolder.transform.rotation;
        //Debug.Log(mouseSpeed);
    }

    void ModelRoatateYClockWise() {
        
        specimenHolder.transform.rotation = Quaternion.AngleAxis(3 * mouseSpeed, Vector3.left) * specimenHolder.transform.rotation;
        //Debug.Log(mouseSpeed);
    }

    void ModelRoatateYCounterClockWise() {
        
        specimenHolder.transform.rotation = Quaternion.AngleAxis(3 * mouseSpeed, Vector3.right) * specimenHolder.transform.rotation;
        //Debug.Log(mouseSpeed);
    }

    void KeyBoardControl()
    {
        specimenHolder = GameObject.Find("SpecimenHolder").transform.GetChild(0).gameObject;


        if (Input.GetKeyDown(KeyCode.Minus))
        {
            ZoomOut();          
        }

        if (Input.GetKeyDown(KeyCode.Equals))
        {
            ZoomIn();
        }

        if (Input.GetKeyDown(KeyCode.UpArrow) && Input.GetKey(KeyCode.LeftShift))
        {
            MoveUp();
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) && Input.GetKey(KeyCode.LeftShift))
        {
            MoveDown();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow) && Input.GetKey(KeyCode.LeftShift))
        {
            MoveLeft();
        }

        if (Input.GetKeyDown(KeyCode.RightArrow) && Input.GetKey(KeyCode.LeftShift))
        {
            MoveRight();
        }

        if (Input.GetKeyDown(KeyCode.UpArrow) && !Input.GetKey(KeyCode.LeftShift))
        {
            ModelRoatateYClockWise();
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) && !Input.GetKey(KeyCode.LeftShift))
        {
            ModelRoatateYCounterClockWise();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.LeftShift))
        {
            ModelRoatateXClockWise();
        }

        if (Input.GetKeyDown(KeyCode.RightArrow) && !Input.GetKey(KeyCode.LeftShift))
        {
            ModelRoatateXCounterClockWise();
        }
    }

    void ControlAssistMore()
    {
        if(comparisonMode.isCompared == false)
        {
            controlAssistExtended.SetActive(true);
            controlAssistFolded.SetActive(false);
        }
   
    }

    void ControlAssistBack()
    {
        controlAssistExtended.SetActive(false);
        controlAssistFolded.SetActive(true);

    }

    void ResetCamera()
    {
        orbitCam.yRotationAxis = 0;
        orbitCam.xRotationAxis = 0;
        orbitCam.xVelocity = 0;
        orbitCam.yVelocity = 0;
    }

    void ResetRotation()
    {
        analysisPage.ResetRotation();
    }

    //Buttons in Comparison Mode

    void ComparedZoomIn() {
        comparisonMode.Tray2ZoomInComparison(0.1f);
    }

    void ComparedZoomOut() {
        comparisonMode.Tray2ZoomOutComparison(0.1f);
    }

    void ComparedModelRoatateXClockWise() {
        var comparedSpecimen = cart.tray2.transform.GetChild(0).GetChild(0).gameObject;
        comparedSpecimen.transform.rotation = Quaternion.AngleAxis(3 * mouseSpeed, Vector3.down) * comparedSpecimen.transform.rotation;
    }

    void ComparedModelRoatateXCounterClockWise() {
        var comparedSpecimen = cart.tray2.transform.GetChild(0).GetChild(0).gameObject;
        comparedSpecimen.transform.rotation = Quaternion.AngleAxis(3 * mouseSpeed, Vector3.up) * comparedSpecimen.transform.rotation;
    }

    void ComparedModelRoatateYClockWise() {
        var comparedSpecimen = cart.tray2.transform.GetChild(0).GetChild(0).gameObject;
        comparedSpecimen.transform.rotation = Quaternion.AngleAxis(3 * mouseSpeed, Vector3.left) * comparedSpecimen.transform.rotation;
    }

    void ComparedModelRoatateYCounterClockWise() {
        var comparedSpecimen = cart.tray2.transform.GetChild(0).GetChild(0).gameObject;
        comparedSpecimen.transform.rotation = Quaternion.AngleAxis(3 * mouseSpeed, Vector3.right) * comparedSpecimen.transform.rotation;
    }

    void ComparedResetRotation() {
        var comparedSpecimen = cart.tray2.transform.GetChild(0).GetChild(0).gameObject;
        comparedSpecimen.transform.rotation = new Quaternion(0, 0, 0, 0);
    }

    //mouse speed control
    public void ChangeMouseSpeed(float newSpeed)
    {
        this.mouseSpeed = newSpeed;
        orbitCam.mouseControlSpeed = newSpeed;
        sliderNum.text = Mathf.Round(mouseSpeed * 44).ToString();
    }


    //add listener for controlling the control assist panel
    void ControlAssistState()
    {
        //Debug.Log("clicked");
        if(isControlAssistOn == false)
        {
            controlAssistFolded.SetActive(!isControlAssistOn);
            controlAssistExtended.SetActive(isControlAssistOn);
        }
        else
        {
            controlAssistFolded.SetActive(!isControlAssistOn);
            controlAssistExtended.SetActive(!isControlAssistOn);
        }

        isControlAssistOn = !isControlAssistOn;

    }
}
