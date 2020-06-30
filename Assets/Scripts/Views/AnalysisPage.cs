﻿using System.Collections;
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

    [Header("Reset Camera position")]
    public Button resetButton;

    private float xPos;
    private float yPos;
    private float zPos;

    [Header("Annotations")]
    public Button toggleAnnotation;
    public GameObject annotationMenu;

    [Header("Proportion Indicator")]
    public ProportionIndicator proportionScript;
    public GameObject proportionIndicator;

    [Header("Other")]

    public GameObject uiObject;
    public StateController stateController;
    public Camera mainCamera;
    public CompareMenu compareMenu;

    
    public void Activate()
    {
        if (stateController.CurrentSpecimenObject == null)
        {
            Debug.LogWarning("Entering analysis view with no subject! Something has gone wrong.");
        }
        compareMenu.gameObject.SetActive(false);
        annotationMenu.gameObject.SetActive(false);
        proportionIndicator.SetActive(true); 
        uiObject.SetActive(true);
        mainCamera.GetComponent<Animator>().enabled = false;
        mainCamera.GetComponent<OrbitCamera>().enabled = true;
        mainCamera.GetComponent<OrbitCamera>().target = stateController.CurrentSpecimenObject.transform;
     
    }

    public void Deactivate()
    {
        proportionScript.ResetProportionIndicator(); // Hide selected specimen on proportion
        proportionIndicator.SetActive(false); 
        uiObject.SetActive(false);
        mainCamera.GetComponent<OrbitCamera>().enabled = false;
        mainCamera.GetComponent<OrbitCamera>().target = null;
    }


    public void Start() {
        
       
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
       // controlAssistant.onClick.AddListener(ToggleController);


        // Annotation Button
        Button toggleAnnotationButton = toggleAnnotation.GetComponent<Button>();
        toggleAnnotationButton.onClick.AddListener(ToggleAnnotations);

        
       

        // Reset Specimen Button

        Button resetCameraPosition = resetButton.GetComponent<Button>();
        resetCameraPosition.onClick.AddListener(ResetCameraPosition);
    }

    public void Update()
    {

        if (stateController.mode != ViewMode.ANALYSIS) return;
        xPos = mainCamera.transform.position.x;
        yPos = mainCamera.transform.position.y;
        zPos = mainCamera.transform.position.z;
       // mainCamera.transform.LookAt(stateController.CurrentSpecimenObject.transform);

        // print(xPos + "  " + yPos + "  " + zPos);

    }

    // CONTROL ASSISTANT BUTTON METHODS

    void ToggleController() {
        print("Toggle Controller On/Off");
        controlAssistantGO.SetActive(!controlAssistantGO.activeInHierarchy);

    }

    void MoveUp() {
        print("UP");
        yPos += 1f;
        mainCamera.transform.position = new Vector3(xPos, yPos, zPos);
    }

    void MoveDown() {
        print("DOWN");
        yPos -= 1f;
        mainCamera.transform.position = new Vector3(xPos, yPos, zPos);
    }

    void MoveLeft() {
        print("LEFT");
        xPos -= 1f;
        mainCamera.transform.position = new Vector3(xPos, yPos, zPos);
    }

    void MoveRight() {
        print("RIGHT");
        xPos += 1f;
        mainCamera.transform.position = new Vector3(xPos, yPos, zPos);
    }

    void ZoomIn() {
        print("Zoom In");
        zPos += 1f;
        mainCamera.transform.position = new Vector3(xPos, yPos, zPos);
    }

    void ZoomOut() {
        print("Zoom Out");
        zPos -= 1f;
        mainCamera.transform.position = new Vector3(xPos, yPos, zPos);
    }

    // RESET BUTTON METHOD

    void ResetCameraPosition() {
        print("Reset");
        mainCamera.transform.position = new Vector3(0, 4, -6);

        mainCamera.transform.rotation = Quaternion.Euler(10, 0, 0);
    }

    // ANNOTATIONS

    void ToggleAnnotations() {
        annotationMenu.SetActive(!annotationMenu.activeInHierarchy);

    }
}
