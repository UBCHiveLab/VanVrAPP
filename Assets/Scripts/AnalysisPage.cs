using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Controller;
using UnityEngine;
using UnityEngine.UI;

public class AnalysisPage : MonoBehaviour, IPage
{
    [Header("Control Assistant")]
    public GameObject controlAssistantGO;
    public Button controllerAssistant;
    public Button controlButtonUp;
    public Button controlButtonDown;
    public Button controlButtonLeft;
    public Button controlButtonRight;
    public Button zoomIn;
    public Button zoomOut;

    [Header("Reset Camera Position")]
    public Button resetButton;

    private float xPos;
    private float yPos;
    private float zPos;

    [Header("Other")]

    public GameObject uiObject;
    public StateController stateController;
    public MSCameraController cameraController;
    public Camera analysisCamera;
    public Camera mainCamera;
    public MSCameraController cameraControllerPrefab;

    public void Activate()
    {
        if (stateController.CurrentSpecimenObject == null)
        {
            Debug.LogWarning("Entering analysis view with no subject! Something has gone wrong.");
        }

        uiObject.SetActive(true);

        // We swap out the analysis camera for the main camera, then spawn in the cameraController
        analysisCamera.transform.position = mainCamera.transform.position;
        analysisCamera.gameObject.SetActive(true);
        mainCamera.gameObject.SetActive(false);
        cameraController = Instantiate(cameraControllerPrefab);
        cameraController.target = stateController.CurrentSpecimenObject.transform;
        cameraController.transform.position = stateController.CurrentSpecimenObject.transform.position;
        MSACC_CameraType cam = new MSACC_CameraType() {
            _camera = analysisCamera, rotationType = MSACC_CameraType.TipoRotac.Orbital, volume = 0.5f
        };
        cameraController.cameras = new[] { cam };
    }

    public void Deactivate()
    {
        // We must swap out the analysis cam and then destroy the cameraController on deactivate,
        // since we cannot swap focuses for the cameraController
        analysisCamera.transform.parent = null;
        analysisCamera.gameObject.SetActive(false);
        mainCamera.gameObject.SetActive(true);

        if (cameraController != null)
        {
            Destroy(cameraController.gameObject);
        }
        uiObject.SetActive(false);
    }


    void Start() {
        // Control Assistant Buttons

        Button controlAssistant = controllerAssistant.GetComponent<Button>();
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
        controlAssistant.onClick.AddListener(ToggleController);

        // Reset Specimen Button

        Button resetCameraPosition = resetButton.GetComponent<Button>();
        resetCameraPosition.onClick.AddListener(ResetCameraPosition);
    }

    void Update()
    {

        if (stateController.mode != ViewMode.ANALYSIS) return;
        xPos = analysisCamera.transform.position.x;
        yPos = analysisCamera.transform.position.y;
        zPos = analysisCamera.transform.position.z;
        mainCamera.transform.LookAt(stateController.CurrentSpecimenObject.transform);

        // print(xPos + "  " + yPos + "  " + zPos);

    }

    // CONTROL ASSISTANT BUTTON METHODS

    void ToggleController() {
        print("Toggle Controller On/Off");
        controlAssistantGO.SetActive(!controlAssistantGO.activeInHierarchy);

        // Change MS camera setting from Orbital to Look At Player when turning on control assistant.
        if (controlAssistantGO.activeInHierarchy)
        {
            cameraController.cameras[0].rotationType = MSACC_CameraType.TipoRotac.LookAtThePlayer;
        }
        else
        {
            cameraController.cameras[0].rotationType = MSACC_CameraType.TipoRotac.Orbital;
        }

    }

    void MoveUp() {
        print("UP");
        yPos += 1f;
        analysisCamera.transform.position = new Vector3(xPos, yPos, zPos);
    }

    void MoveDown() {
        print("DOWN");
        yPos -= 1f;
        analysisCamera.transform.position = new Vector3(xPos, yPos, zPos);
    }

    void MoveLeft() {
        print("LEFT");
        xPos -= 1f;
        analysisCamera.transform.position = new Vector3(xPos, yPos, zPos);
    }

    void MoveRight() {
        print("RIGHT");
        xPos += 1f;
        analysisCamera.transform.position = new Vector3(xPos, yPos, zPos);
    }

    void ZoomIn() {
        print("Zoom In");
        zPos += 1f;
        analysisCamera.transform.position = new Vector3(xPos, yPos, zPos);
    }

    void ZoomOut() {
        print("Zoom Out");
        zPos -= 1f;
        analysisCamera.transform.position = new Vector3(xPos, yPos, zPos);
    }

    // RESET BUTTON METHOD

    void ResetCameraPosition() {
        print("Reset");
        analysisCamera.transform.position = new Vector3(0, 4, -6);

        analysisCamera.transform.rotation = Quaternion.Euler(10, 0, 0);
    }
}
