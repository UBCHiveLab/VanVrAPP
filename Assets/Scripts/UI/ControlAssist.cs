using UnityEngine;
using UnityEngine.UI;

public class ControlAssist : MonoBehaviour
{

    public Button controlButtonUp;
    public Button controlButtonDown;
    public Button controlButtonLeft;
    public Button controlButtonRight;
    public Button controlButtonZoomIn;
    public Button controlButtonZoomOut;

    private OrbitCamera _orbitCamera;

    void Start()
    {
        controlButtonUp.onClick.AddListener(Up);
        controlButtonDown.onClick.AddListener(Down);
        controlButtonLeft.onClick.AddListener(Left);
        controlButtonRight.onClick.AddListener(Right);
        controlButtonZoomIn.onClick.AddListener(ZoomIn);
        controlButtonZoomOut.onClick.AddListener(ZoomOut);

    }

    void OnEnable()
    {
        _orbitCamera = Camera.main.GetComponent<OrbitCamera>();
        _orbitCamera.controlAssistActive = true;
    }

    void OnDisable()
    {
        _orbitCamera.controlAssistActive = false;
    }

    void Up() {
        //_orbitCamera.doSomething()
        /*
        print("UP");
        yPos += 1f;
        mainCamera.transform.position = new Vector3(xPos, yPos, zPos);
        */
    }

    void Down() {
        //print("DOWN");
        //yPos -= 1f;
        //mainCamera.transform.position = new Vector3(xPos, yPos, zPos);
    }

    void Left() {
        //print("LEFT");
        //xPos -= 1f;
        //mainCamera.transform.position = new Vector3(xPos, yPos, zPos);
    }

    void Right() {
        //print("RIGHT");
        //xPos += 1f;
        //mainCamera.transform.position = new Vector3(xPos, yPos, zPos);
    }

    void ZoomIn() {
        //print("Zoom In");
        //zPos += 1f;
        //mainCamera.transform.position = new Vector3(xPos, yPos, zPos);
    }

    void ZoomOut() {
        //print("Zoom Out");
        //zPos -= 1f;
        //mainCamera.transform.position = new Vector3(xPos, yPos, zPos);
    }

}
