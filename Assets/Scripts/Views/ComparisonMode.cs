using Assets.Scripts.State;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComparisonMode : MonoBehaviour
{
    [Header("State")]
    public GameObject comparisonCanvas;
    public GameObject analysisLeftContainer;
    public GameObject comparisonBackground;
    public GameObject controlAssistLeft;
    public GameObject analysisTopLeftContainer;
    public GameObject analysisTopRightFocusMode;
    public GameObject annotationDisplayController;
    public bool isControlAssistLeftShow;
    public bool isCompared = false;

    [Header("button")]
    public Button comparisonCloseButton;
    public Button comparedResetRotation;

    [Header("External")]
    public SpecimenCart cart;
    public Camera mainCamera;
    public OrbitCamera orbitCam;
    public AnalysisPage analysisPage;
    public ControlAssist controlAssist;
    //public TrayPage trayPage;
    //public StateController stateController;


    //public void Activate() { }

    //public void Deactivate() { }

    // Start is called before the first frame update
    void Start()
    {
        //addlistener
        comparisonCloseButton.onClick.AddListener(ToggleClose);
        comparedResetRotation.onClick.AddListener(ComparedResetRotation);

        //
        analysisPage = GameObject.Find("UIManager").GetComponent<AnalysisPage>();
        orbitCam = mainCamera.GetComponent<OrbitCamera>();
        //controlAssist = GameObject.Find("Control_Assistant").GetComponent<ControlAssist>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Comparison State; activated by button in analysis mode
    public void ComparisonState()
    {
        // Check compared tray exsit or not
        if (cart.tray1 == null || cart.tray2 == null) return;

        isCompared = !isCompared;

        comparisonCanvas.SetActive(isCompared);
        comparisonBackground.SetActive(isCompared);
        analysisLeftContainer.SetActive(!isCompared);
        analysisTopLeftContainer.SetActive(!isCompared);
        analysisTopRightFocusMode.SetActive(!isCompared);

        //Note: Improve later. When annotaion is on, force to close after clicking comparison mode
        analysisPage.ToggleAnnotations(false);
        annotationDisplayController.SetActive(false);
        ControlAssistLeftState();

        //Set main camera positon in comparison mode
        if (isCompared == true)
        {   
            mainCamera.GetComponent<OrbitCamera>().enabled = !isCompared;
            cart.ComprisonModePosition();

            // set camera to tray view perspective; set const for these later
            mainCamera.transform.position = new Vector3(-0.17f, 2.2f, 20.34f);
            mainCamera.transform.rotation = new Quaternion(0,0,0,0);
        }
        else
        {
            mainCamera.GetComponent<OrbitCamera>().enabled = !isCompared;
            cart.SpecimenPositionTrayView();
        }

    }

    //Zoom in/out in comparison mode controlled by ui button; might improve by looking at camera 
    public void Tray1ZoomInComparison(float num)
    {
        //sin(20) cos(20)
        if (cart.tray1.transform.localPosition.x <= -0.42)
        {
            cart.tray1.transform.localPosition = new Vector3(-0.4210001f, 2.25f, -0.02999993f);
        }
        //Debug.Log(cart.tray1.transform.localPosition);
        cart.tray1.transform.localPosition -= new Vector3(num * 0.342f * controlAssist.mouseSpeed, 0, num * 0.94f * controlAssist.mouseSpeed);
        //Debug.Log(cart.tray1.transform.localPosition);
    }

    public void Tray1ZoomOutComparison(float num)
    {
        if(cart.tray1.transform.localPosition.x >= -0.2359999)
        {
            cart.tray1.transform.localPosition = new Vector3(-0.02359999f, 2.25f, 1.252f);
        }

        Debug.Log(cart.tray1.transform.localPosition);
        cart.tray1.transform.localPosition += new Vector3(num * 0.342f * controlAssist.mouseSpeed, 0, num * 0.94f * controlAssist.mouseSpeed);
        Debug.Log(cart.tray1.transform.localPosition);
    }

    public void Tray2ZoomInComparison(float num) {
        //sin(30) cos(30)
        if (cart.tray2.transform.localPosition.x >= -1.35)
        {
            cart.tray2.transform.localPosition = new Vector3(-1.35f, 2.25f, 0.2402f);
        }

        cart.tray2.transform.localPosition += new Vector3(num * 0.5f * controlAssist.mouseSpeed, 0, -num * 0.866f * controlAssist.mouseSpeed);
    }

    public void Tray2ZoomOutComparison(float num) {
        if (cart.tray2.transform.localPosition.x <= -2.349999)
        {
            cart.tray2.transform.localPosition = new Vector3(-2.349999f, 2.25f, 1.9722f);
        }

        cart.tray2.transform.localPosition -= new Vector3(num * 0.5f * controlAssist.mouseSpeed, 0, -num * 0.866f * controlAssist.mouseSpeed);

    }

    //Close the comparison mode
    void ToggleClose()
    {
        analysisPage.ToggleCompare();
    }

    void ControlAssistLeftState()
    {
        controlAssistLeft.SetActive(isControlAssistLeftShow);
    }

    void ComparedResetRotation()
    {
        var comparedSpecimen = cart.tray2.transform.GetChild(0).GetChild(0).gameObject;
        comparedSpecimen.transform.rotation = new Quaternion(0, 0, 0, 0);
    }
}
