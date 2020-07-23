using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//These are triggered by animation events
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
