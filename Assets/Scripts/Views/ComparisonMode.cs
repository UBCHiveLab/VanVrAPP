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

    void ToggleClose()
    {
        analysisPage.ToggleCompare();
    }

    void ControlAssistLeftState()
    {
        controlAssistLeft.SetActive(controlAssist.isControlAssistOn);
    }
}
