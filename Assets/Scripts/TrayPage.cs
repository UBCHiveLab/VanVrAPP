using UnityEngine;

public class TrayPage : MonoBehaviour, IPage
{
    public GameObject uiObject;


    public void Activate() {
        uiObject.SetActive(true);
    }

    public void Deactivate() {
        uiObject.SetActive(false);
    }
}
