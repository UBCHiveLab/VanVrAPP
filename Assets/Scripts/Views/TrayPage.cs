﻿using Assets.Scripts.Controller;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TrayPage : MonoBehaviour, IPage
{
    public StateController stateController;
    public SelectorMenu selectorMenu;
    public GameObject uiObject;
    public GameObject actionButtons;
    public Button compareButton;
    public Button analyzeButton;
    public ProportionIndicator proportionScript;
    public bool selectingCompareSpecimen;
    public Button shelfToggle;

    public FocusDistanceFinder focusDistanceFinder;

    // TEMP: we probably want to animate camera back to tray when finished
    public bool camSet;
    private Vector3 camDefaultPosition = new Vector3(0.22f, 1.91f, 20.04f);
    private Vector3 camDefaultRotation = new Vector3(-4.211f, 0f, 0f);
    private float camDefaultFov = 60f;

    public SpecimenCart cart;

    void Start()
    {
        compareButton.onClick.AddListener(SelectCompare);
        analyzeButton.onClick.AddListener(SelectAnalysis);
        shelfToggle.onClick.AddListener(ToggleShelfMenu); 
        actionButtons.SetActive(false);

    }

    public void Activate() {
        selectorMenu.gameObject.SetActive(false);
        focusDistanceFinder.enabled = true;
        uiObject.SetActive(true);

        // TEMP: use animation
        if (camSet)
        {
            shelfToggle.animator.SetBool("Hidden", false);
            Camera.main.transform.position = camDefaultPosition;
            Camera.main.transform.rotation = Quaternion.Euler(camDefaultRotation);
            Camera.main.fieldOfView = camDefaultFov;
        }

        // If no current specimen...
        if (ReferenceEquals(stateController.CurrentSpecimenData, null))
        {
            // ... hide the action buttons, turn off compare if relevant.
            actionButtons.SetActive(false);
            if (selectingCompareSpecimen) {
                CompareOff();
            }
        } else {
            // Else put compare mode on
            if (stateController.CompareSpecimenData != null)
            {
                CompareOn();
            }
            else
            {
                CompareOff();
            }
        }

    }

    public void Deactivate() {
        uiObject.SetActive(false);
        shelfToggle.gameObject.SetActive(true);
    }

    public void SpecimenSelected(SpecimenData data)
    {
        analyzeButton.interactable = false;

        if (selectingCompareSpecimen)
        {
            StartCoroutine(stateController.AddCompareSpecimen(data, OnAddCompareSpecimen));
        } else
        {
            StartCoroutine(stateController.AddPrimarySpecimen(data, OnAddPrimarySpecimen));
            //cart.AddSpecimenPrimary(specimen);
        }
        actionButtons.SetActive(true);

    }

    private void OnAddPrimarySpecimen(GameObject obj)
    {
        cart.AddSpecimenPrimary(obj);
        analyzeButton.interactable = true;
    }

    private void OnAddCompareSpecimen(GameObject obj) {
        cart.AddSpecimenCompare(obj);
        analyzeButton.interactable = true;
    }

    public void HaveArrivedAtTray()
    {
        shelfToggle.animator.SetBool("Hidden", false);
    }

    public void HaveEnteredFromLandingPage()
    {
        shelfToggle.animator.SetBool("Hidden", true);
    }

    public void SelectAnalysis()
    {
        // TODO: THIS is the source of the camera bug. change later when we have final animations
        /*camDefaultPosition = Camera.main.transform.position;
        camDefaultRotation = Camera.main.transform.rotation.eulerAngles;
        camDefaultFov = Camera.main.fieldOfView;*/
        camSet = true;

        if (stateController.CompareSpecimenObject == null)
        {
            cart.RemoveTray2();
        }
        proportionScript.HighlightProportionIndicator(); // Show proportion indicator
        stateController.mode = ViewMode.ANALYSIS;
    }

    public void SelectCompare()
    {
        if (!selectingCompareSpecimen)
        {
            CompareOn();
        }
        else
        {
            CompareOff();
        }
    }

    public void ToggleShelfMenu()
    {
        bool showMenu = !selectorMenu.gameObject.activeSelf;
        selectorMenu.gameObject.SetActive(showMenu);

        shelfToggle.animator.SetBool("Hidden", showMenu);
        //shelfToggle.gameObject.SetActive(!showMenu);
    }

    private void CompareOff()
    {
        selectingCompareSpecimen = false;
        stateController.RemoveCompareSpecimen();
        compareButton.GetComponent<UITwoStateIndicator>().UpdateState(false);
        selectorMenu.EndCompare();
        cart.RemoveTray2();
    }

    private void CompareOn()
    {
        selectingCompareSpecimen = true;
        selectorMenu.SelectCompare();
        compareButton.GetComponent<UITwoStateIndicator>().UpdateState(true);
        cart.SpawnTray2();
    }
   


}
