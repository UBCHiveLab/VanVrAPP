using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GeneralSettingsPanel : MonoBehaviour
{
    [Header("InternalStructure")]
    public Slider volumeSlider;
    public TextMeshProUGUI volumeLabel;
    public Transform displayParent;
    public Toggle windowedMode;

    [Header("Prefabs")] public DisplayResolutionButton displayResButtonPrefab;

    [Header("Services")]
    public GeneralSettings settings;
    public ConfirmationPanel ConfirmationPanel;


    private int currentResolutionButtonIndex;
    private List<DisplayResolutionButton> resoButtons = new List<DisplayResolutionButton>();


    void OnEnable() {
        UpdateVolume();
        UpdateRes();
    }

    // Start is called before the first frame update
    void Start()
    {
        currentResolutionButtonIndex = settings.GetCurrentResolutionIndex();

        for (int i = 0; i < settings.displayResolutions.Length; i++) {
            DisplayResolutionButton btn = Instantiate(displayResButtonPrefab, displayParent);
            resoButtons.Add(btn);
            bool active = i == currentResolutionButtonIndex;

            DisplayResolutionButtonState state =
                active ? DisplayResolutionButtonState.ACTIVE : DisplayResolutionButtonState.AVAILABLE;
            if (!settings.supportedResolutions.Contains(settings.displayResolutions[i]))
                state = DisplayResolutionButtonState.UNAVAILABLE;

            btn.Populate(settings.displayResolutions[i], state, this);
        }

        windowedMode.isOn = settings.windowed;
    }

    public void OnChangeVolume(float val)
    {
        settings.OnChangeVolume(val);
        UpdateVolume();
    }

    public void OnChangeResolution(DisplayResolutionButton btn)
    {
        settings.OnChangeResolution(btn.resolution, windowedMode.isOn);
        UpdateRes();
    }

    public void OnChangeWindowed(bool windowed)
    {
        settings.OnChangeResolution(settings.currentRes, windowed);
    }

    public void UpdateRes()
    {
        windowedMode.isOn = settings.windowed;
        currentResolutionButtonIndex = settings.GetCurrentResolutionIndex();

        if (resoButtons.Count > 0)
        {
            for (int i = 0; i < settings.displayResolutions.Length; i++) {
                bool active = i == currentResolutionButtonIndex;

                DisplayResolutionButtonState state =
                    active ? DisplayResolutionButtonState.ACTIVE : DisplayResolutionButtonState.AVAILABLE;
                if (!settings.supportedResolutions.Contains(settings.displayResolutions[i]))
                    state = DisplayResolutionButtonState.UNAVAILABLE;

                resoButtons[i].Populate(settings.displayResolutions[i], state, this);
            }
        }

    }

    public void UpdateVolume()
    {
        volumeLabel.text = Mathf.RoundToInt(settings.currentVolume * 100f).ToString() + "%";
        volumeSlider.value = settings.currentVolume;
    }

    public void Quit() {
        ConfirmationPanel.Populate("Are you sure you want to exit the program?", "Quit?", null, Application.Quit);
    }

    public void ClearCache() {
        ConfirmationPanel.Populate(
            "Do you want to clear the cache? This frees up space and may fix some issues with specimens, but download times may be longer.",
            "Clear Cache?",
            null, () => Caching.ClearCache()
        );
    }
}
