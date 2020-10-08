using UnityEngine;
using UnityEngine.UI;
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

    private OrbitCamera orbitCam;
    private GameObject specimenHolder;

    void Start()
    {
        //controlButtonUp.onClick.AddListener(MoveUp);
        //controlButtonDown.onClick.AddListener(MoveDown);
        //controlButtonLeft.onClick.AddListener(MoveLeft);
        //controlButtonRight.onClick.AddListener(MoveRight);
        controlButtonZoomIn.onClick.AddListener(ZoomIn);
        controlButtonZoomOut.onClick.AddListener(ZoomOut);

    }

    void Update()
    {
        KeyBoardControl();
        
    }

    void OnEnable()
    {
        orbitCam = Camera.main.GetComponent<OrbitCamera>();
        orbitCam.controlAssistActive = true;
    }

    void OnDisable()
    {
        orbitCam.controlAssistActive = false;
    }


    void MoveUp() {
        orbitCam.yVelocity += 1f * orbitCam.rotationSensitivity;
        orbitCam.yRotationAxis += orbitCam.yVelocity;
        orbitCam.yRotationAxis = orbitCam.ClampAngleBetweenMinAndMax(orbitCam.yRotationAxis, orbitCam.rotationLimit.x, orbitCam.rotationLimit.y);
    }

    void MoveDown() {
        orbitCam.yVelocity -= 1f * orbitCam.rotationSensitivity;
        orbitCam.yRotationAxis += orbitCam.yVelocity;
        orbitCam.yRotationAxis = orbitCam.ClampAngleBetweenMinAndMax(orbitCam.yRotationAxis, orbitCam.rotationLimit.x, orbitCam.rotationLimit.y);

    }

    void MoveLeft() {
        orbitCam.xVelocity += 6f * orbitCam.rotationSensitivity;
        orbitCam.yRotationAxis += orbitCam.yVelocity;

    }

    void MoveRight() {
        orbitCam.xVelocity -= 6f * orbitCam.rotationSensitivity;
        orbitCam.yRotationAxis += orbitCam.yVelocity;

    }

    void ZoomIn() {
        orbitCam.DoZoom(1f, Time.deltaTime);

    }

    void ZoomOut() {
        orbitCam.DoZoom(-1f, Time.deltaTime);

    }

    void ModelRoatateXClockWise() {
        specimenHolder.transform.Rotate(5f, 0, 0);       
    }

    void ModelRoatateXCounterClockWise() {
        specimenHolder.transform.Rotate(-5f, 0, 0);
    }

    void ModelRoatateYClockWise() {
        specimenHolder.transform.Rotate(0, 5f, 0);
    }

    void ModelRoatateYCounterClockWise() {       
        specimenHolder.transform.Rotate(0, -5f, 0);
    }

    void KeyBoardControl()
    {
        specimenHolder = GameObject.Find("SpecimenHolder");

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
            ModelRoatateXClockWise();
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) && !Input.GetKey(KeyCode.LeftShift))
        {
            ModelRoatateXCounterClockWise();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.LeftShift))
        {
            ModelRoatateYClockWise();
        }

        if (Input.GetKeyDown(KeyCode.RightArrow) && !Input.GetKey(KeyCode.LeftShift))
        {
            ModelRoatateYCounterClockWise();
        }
    }

}
