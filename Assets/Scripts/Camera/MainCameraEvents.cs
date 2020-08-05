using UnityEngine;


// For triggering camera events from animations. (Currently only used for the Landing -> Tray transition).

public class MainCameraEvents : MonoBehaviour
{
    public TrayPage tray;

    public void OnEnterFromLandingPage()
    {
        tray.HaveEnteredFromLandingPage();
    }

    public void OnArrivedAtTray()
    {
        tray.HaveArrivedAtTray();
    }
}
