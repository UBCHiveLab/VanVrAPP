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
    public bool isCompared = false;

    [Header("button")]
    public Button comparisonCloseButton;

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

        //
        analysisPage = GameObject.Find("UIManager").GetComponent<AnalysisPage>();
        orbitCam = mainCamera.GetComponent<OrbitCamera>();
        //controlAssist = GameObject.Find("Control_Assistant").GetComponent<ControlAssist>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ComparisonState()
    {
        if (cart.tray1 == null || cart.tray2 == null) return;

        isCompared = !isCompared;

        comparisonCanvas.SetActive(isCompared);
        comparisonBackground.SetActive(isCompared);
        analysisLeftContainer.SetActive(!isCompared);
        ControlAssistLeftState();



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

    //Zoom in/out in comparison mode
    public void Tray1ZoomInComparison(float num)
    {
        //sin(20) cos(20)
        if (cart.tray1.transform.localPosition.x <= -0.52)
        {
            cart.tray1.transform.localPosition = new Vector3(-0.5236001f, 2.25f, -0.2520001f);
        }
        //Debug.Log(cart.tray1.transform.localPosition);
        cart.tray1.transform.localPosition -= new Vector3(num * 0.342f, 0, num * 0.94f);
        //Debug.Log(cart.tray1.transform.localPosition);
    }

    public void Tray1ZoomOutComparison(float num)
    {
        if(cart.tray1.transform.localPosition.x >= -0.23)
        {
            cart.tray1.transform.localPosition = new Vector3(0.02359999f, 2.25f, 1.252f);
        }

        Debug.Log(cart.tray1.transform.localPosition);
        cart.tray1.transform.localPosition += new Vector3(num * 0.342f, 0, num * 0.94f);
        Debug.Log(cart.tray1.transform.localPosition);
    }

    public void Tray2ZoomInComparison(float num) {
        //sin(30) cos(30)
        if (cart.tray2.transform.localPosition.x >= -1.35)
        {
            cart.tray2.transform.localPosition = new Vector3(-1.35f, 2.25f, 0.2402f);
        }

        cart.tray2.transform.localPosition += new Vector3(num * 0.5f, 0, -num * 0.866f);
    }

    public void Tray2ZoomOutComparison(float num) {
        if (cart.tray2.transform.localPosition.x <= -2.349999)
        {
            cart.tray2.transform.localPosition = new Vector3(-2.349999f, 2.25f, 1.9722f);
        }

        cart.tray2.transform.localPosition -= new Vector3(num * 0.5f, 0, -num * 0.866f);

    }

    void ToggleClose()
    {
        analysisPage.ToggleCompare();
    }

    void ControlAssistLeftState()
    {
        controlAssistLeft.SetActive(controlAssist.isControlAssistOn);
    }
}
