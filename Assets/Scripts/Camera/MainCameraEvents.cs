using Assets.Scripts.State;
using UnityEngine;


// For triggering camera events from animations. (Currently only used for the Landing -> Tray transition).

public class MainCameraEvents : MonoBehaviour
{
    public GameObject canvas;
    public TrayPage tray;
    public StateController stateController;

    public void OnEnterFromLandingPage()
    {
        // disable the canvas while traveling from the landing page to the tray. This stops Unity from trying
        // to update the canvas while the "walking to the tray" animation is occuring, which saves CPU time
        canvas.SetActive(false);
    }

    public void OnArrivedAtTray()
    {
        // allow the user to interact with the UI canvas again, as the "walking to tray" animation is done
        canvas.SetActive(true);
        tray.HaveArrivedAtTray();
    }
}
