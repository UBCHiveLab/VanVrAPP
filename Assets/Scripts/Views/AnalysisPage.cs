﻿using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Controller;
using TMPro;
using UnityEngine;
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

    [Header("Other")]
    public TextMeshProUGUI targetSpecimenLabel;
    public GameObject uiObject;
    public StateController stateController;
    public Camera mainCamera;
    public CompareMenu compareMenu;
    public AnnotationDisplay annotationDisplay;

    
    public void Activate()
    {
        if (stateController.CurrentSpecimenObject == null)
        {
            Debug.LogWarning("Entering analysis view with no subject! Something has gone wrong.");
        }

        controlAssistant.gameObject.SetActive(false);
        compareMenu.gameObject.SetActive(false);
        ToggleAnnotations(false);
        uiObject.SetActive(true);
        mainCamera.GetComponent<Animator>().enabled = false;
        mainCamera.GetComponent<OrbitCamera>().enabled = true;
        mainCamera.GetComponent<OrbitCamera>().target = stateController.CurrentSpecimenObject.transform;

        targetSpecimenLabel.text = stateController.CurrentSpecimenData.name;

        //annotationDisplay.Activate();
    }

    public void Deactivate()
    {
        uiObject.SetActive(false);
        mainCamera.GetComponent<OrbitCamera>().enabled = false;
        mainCamera.GetComponent<OrbitCamera>().target = null;
    }


    void Start() {
        // Control Assistant Buttons
        controlAssistToggle.Bind((on) => controlAssistant.gameObject.SetActive(!controlAssistant.gameObject.activeSelf));


        // Annotation Button
        annotationToggle.Bind(ToggleAnnotations);

        // Reset Specimen Button
        resetButton.onClick.AddListener(ResetCameraPosition);

        // Compare toggle
        compareButton.onClick.AddListener(() => compareMenu.TogglePanel());
    }

    void Update()
    {

        if (stateController.mode != ViewMode.ANALYSIS) return;

    }


    // RESET BUTTON METHOD

    void ResetCameraPosition() {
        print("Reset");
        mainCamera.transform.position = new Vector3(0, 4, -6);

        mainCamera.transform.rotation = Quaternion.Euler(10, 0, 0);
    }

    // ANNOTATIONS

    void ToggleAnnotations(bool on) {
        annotationDisplay.gameObject.SetActive(on);
        if (annotationDisplay.gameObject.activeSelf)
        {
            annotationDisplay.Activate();
        }
        else
        {
            annotationDisplay.Deactivate();
        }
    }
}
