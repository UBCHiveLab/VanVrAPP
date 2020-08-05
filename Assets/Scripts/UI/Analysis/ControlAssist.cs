using UnityEngine;
using UnityEngine.UI;

public class ControlAssist : MonoBehaviour
{
    [Header("Buttons")]
    public Button controlButtonUp;
    public Button controlButtonDown;
    public Button controlButtonLeft;
    public Button controlButtonRight;
    public Button controlButtonZoomIn;
    public Button controlButtonZoomOut;

    private OrbitCamera orbitCam;

    void Start()
    {
        controlButtonUp.onClick.AddListener(MoveUp);
        controlButtonDown.onClick.AddListener(MoveDown);
        controlButtonLeft.onClick.AddListener(MoveLeft);
        controlButtonRight.onClick.AddListener(MoveRight);
        controlButtonZoomIn.onClick.AddListener(ZoomIn);
        controlButtonZoomOut.onClick.AddListener(ZoomOut);

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

}
