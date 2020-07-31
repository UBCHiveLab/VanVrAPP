using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Slider = UnityEngine.UI.Slider;

public class GeneralSettings : MonoBehaviour
{
    [Header("InternalStructure")]
    public Slider volumeSlider;
    public TextMeshProUGUI volumeLabel;
    public Transform displayParent;
    public Toggle windowedMode;

    [Header("ExternalStructure")]
    public NotificationPanel notificationPanel;
    public AudioSource[] controlledAudioSources;

    [Header("Options")] public Vector2Int[] displayResolutions;

    [Header("Prefabs")] public DisplayResolutionButton displayResButtonPrefab;

    [Header("Values")]
    private Vector2Int currentRes;
    private float currentVolume = 0.5f;
    private DisplayResolutionButton clickedButton;

    void Start()
    {
        volumeSlider.value = currentVolume;
        volumeLabel.text = Mathf.RoundToInt(currentVolume * 100f).ToString() + "%";

        for (int i = 0; i < displayResolutions.Length; i++)
        {
            DisplayResolutionButton btn = Instantiate(displayResButtonPrefab, displayParent);
            bool active = new Vector2Int(Screen.width, Screen.height) == displayResolutions[i];
            btn.Populate(displayResolutions[i], active, this);
            if (active)
            {
                clickedButton = btn;
            }
        }

        windowedMode.isOn = !Screen.fullScreen;
        OnChangeVolume(currentVolume);
    }


    public Vector2 GetResolution()
    {
        return currentRes;
    }

    public float GetSoundVolume()
    {
        return currentVolume;
    }

    public void OnChangeVolume(float val)
    {
        currentVolume = val;
        volumeLabel.text = Mathf.RoundToInt(currentVolume * 100f).ToString() + "%";
        volumeSlider.value = val;

        foreach (AudioSource src in controlledAudioSources)
        {
            src.volume = val;
        }
    }
        
    public void OnChangeResolution(Vector2Int res, DisplayResolutionButton btn)
    {
        clickedButton.button.interactable = true;
        currentRes = res;
        Screen.SetResolution(currentRes.x, currentRes.y, !windowedMode.isOn);
        clickedButton = btn;
        btn.button.interactable = false;

    }

    public void OnChangeWindowed(bool windowed)
    {
        Screen.SetResolution(currentRes.x, currentRes.y, !windowedMode.isOn);
    }

    public void Quit()
    {
        notificationPanel.Populate("Are you sure you want to exit the program?", null, Application.Quit);
    }

    public void ClearCache()
    {
        notificationPanel.Populate(
            "Do you want to clear the cache? This frees up space and may fix some issues with specimens, but download times may be longer.",
            null, () => Caching.ClearCache()
    );
}


}
