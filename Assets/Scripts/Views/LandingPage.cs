using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Controller;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class LandingPage : MonoBehaviour, IPage
{
    public StateController stateController;
    public GameObject uiObject;
    public Button startButton;
    public GameObject doorL;
    public GameObject doorR;
    public GameObject mainCamera;
    public GameObject disclaimerPanel;
    public Animator mainCameraAnimator;
    public AudioSource startAudio;
    public PostProcessVolume volume;
    public FocusDistanceFinder focusDistanceFinder;

    private DepthOfField depthOfField;


    void Start()
    {
        Button btn = startButton.GetComponent<Button>();
        btn.onClick.AddListener(StartSession);
        mainCameraAnimator = Camera.main.GetComponent<Animator>();

        volume.profile.TryGetSettings(out depthOfField);

    }

    void StartSession()
    {
        startAudio.GetComponent<AudioSource>().Play(0);
        doorL.GetComponent<Animator>().SetTrigger("Start");
        doorR.GetComponent<Animator>().SetTrigger("Start");
        mainCameraAnimator.SetTrigger("Start");
        stateController.mode = ViewMode.TRAY;
        depthOfField.active = false;


    }

    public void Activate()
    {
        focusDistanceFinder.enabled = false;
        uiObject.SetActive(true);
        disclaimerPanel.SetActive(true);
        depthOfField.active = true;
        depthOfField.focusDistance.value = 1f;
    }

    public void Deactivate()
    {
        uiObject.SetActive(false);
    }
}