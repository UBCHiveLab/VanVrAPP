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

    [Header("Control Assist Panel")]

    public GameObject controlAssistFolded;
    public GameObject controlAssistExtended;
    public bool isControlAssistOn = true;

    [Header("Slider")]

    public float mouseSpeed;
    public TextMeshProUGUI sliderNum;

    private OrbitCamera orbitCam;
    private AnalysisPage analysisPage;
    private ComparisonMode comparisonMode;
    private GameObject specimenHolder;
    

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
        orbitCam.DoZoom(1f, Time.deltaTime);


    }

    void ZoomOut() {
        orbitCam.DoZoom(-1f, Time.deltaTime);

    }

    void ModelRoatateXClockWise() {
        
        specimenHolder.transform.rotation = Quaternion.AngleAxis(3 * mouseSpeed, Vector3.down) * specimenHolder.transform.rotation;
        Debug.Log(mouseSpeed);
    }

    void ModelRoatateXCounterClockWise() {
        
        specimenHolder.transform.rotation = Quaternion.AngleAxis(3 * mouseSpeed, Vector3.up) * specimenHolder.transform.rotation;
        Debug.Log(mouseSpeed);
    }

    void ModelRoatateYClockWise() {
        
        specimenHolder.transform.rotation = Quaternion.AngleAxis(3 * mouseSpeed, Vector3.left) * specimenHolder.transform.rotation;
        Debug.Log(mouseSpeed);
    }

    void ModelRoatateYCounterClockWise() {
        
        specimenHolder.transform.rotation = Quaternion.AngleAxis(3 * mouseSpeed, Vector3.right) * specimenHolder.transform.rotation;
        Debug.Log(mouseSpeed);
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
