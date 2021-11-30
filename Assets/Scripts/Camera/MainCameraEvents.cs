using Assets.Scripts.State;
using UnityEngine;


// For triggering camera events from animations. (Currently only used for the Landing -> Tray transition).

public class MainCameraEvents : MonoBehaviour
{
    public GameObject canvas;
    public GameObject displayCanvas;
    public TrayPage tray;
    public StateController stateController;
    public Animator cameraAnimation;
    public Camera maincamera;
    public Camera displayCamera;
    public GameObject uiSkipButton;

    public void Start()
    {
        maincamera.enabled = true;
        displayCamera.enabled = true;
        tray.ToggleShelfMenu();
        cameraAnimation.GetComponent<Animator>().enabled = false;
        maincamera.transform.position = new Vector3(0.22f, 1.91f, 20.04f);
        uiSkipButton.SetActive(false);
        OnArrivedAtTray();
    }
    public void OnEnterFromLandingPage()
    {
        // disable the canvas while traveling from the landing page to the tray. This stops Unity from trying
        // to update the canvas while the "walking to the tray" animation is occuring, which saves CPU time
        canvas.SetActive(false);
        displayCanvas.SetActive(false);
        uiSkipButton.SetActive(true);
    }

    public void OnArrivedAtTray()
    {
        // allow the user to interact with the UI canvas again, as the "walking to tray" animation is done
        canvas.SetActive(true);
        displayCanvas.SetActive(true);
        tray.HaveArrivedAtTray();
        uiSkipButton.SetActive(false);
    }

    public void SkipEnterAnimation()
    {
        /*
        cameraAnimation.GetComponent<Animator>().enabled = false;
        maincamera.transform.position = new Vector3(0.22f, 1.91f, 20.04f);
        uiSkipButton.SetActive(false);
        OnArrivedAtTray();
        */
    }
}
