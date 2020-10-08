﻿using UnityEngine;
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
    public Button controlButtonMore;
    public Button controlButtonBack;
    public Button controlButtonResetCam;
    public Button controlButtonResetRotation;

    [Header("Control Assist Panel")]

    public GameObject controlAssistFolded;
    public GameObject controlAssistExtended;

    private OrbitCamera orbitCam;
    private AnalysisPage analysisPage;
    private GameObject specimenHolder;

    void Start()
    {
        controlButtonXClockWise.onClick.AddListener(ModelRoatateXClockWise);
        controlButtonXCounterClockWise.onClick.AddListener(ModelRoatateXCounterClockWise);
        controlButtonYClockWise.onClick.AddListener(ModelRoatateYClockWise);
        controlButtonYCounterClockWise.onClick.AddListener(ModelRoatateXCounterClockWise);
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

    }

    void Update()
    {
        KeyBoardControl();
        
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
        specimenHolder.transform.Rotate(0, 5f, 0);       
    }

    void ModelRoatateXCounterClockWise() {
        specimenHolder.transform.Rotate(0, -5f, 0);
    }

    void ModelRoatateYClockWise() {
        specimenHolder.transform.Rotate(5f, 0, 0);
    }

    void ModelRoatateYCounterClockWise() {       
        specimenHolder.transform.Rotate(-5f, 0, 0);
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
        controlAssistExtended.SetActive(true);
        controlAssistFolded.SetActive(false);

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

}
