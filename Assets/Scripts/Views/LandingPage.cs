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
    public GameObject doorL;
    public GameObject doorR;
    public DisclaimerPanel disclaimerPanel;
    public Animator mainCameraAnimator;
    public AudioSource startAudio;
    public PostProcessVolume volume;
    public FocusDistanceFinder focusDistanceFinder;
    public StartButton holdToStartButton;
    public PostProcessingBlender blender;


    void Start()
    {
        mainCameraAnimator = Camera.main.GetComponent<Animator>();

        disclaimerPanel.gameObject.SetActive(false);
        disclaimerPanel.enterAction = StartSession;
        disclaimerPanel.backAction = DismissDisclaimer;
        focusDistanceFinder.enabled = false;


        if (holdToStartButton == null)
        {
            Debug.LogWarning("Please bind a holdToStartButton in LandingPage");
        }
        else
        {
            holdToStartButton.bindAction(EntranceClicked);
        }
    }

    void StartSession()
    {
        startAudio.GetComponent<AudioSource>().Play(0);
        doorL.GetComponent<Animator>().SetTrigger("Start");
        doorR.GetComponent<Animator>().SetTrigger("Start");
        mainCameraAnimator.SetTrigger("Start");
        stateController.mode = ViewMode.TRAY;
        //depthOfField.active = false;
        disclaimerPanel.StartDismiss();
    }

    public void DismissDisclaimer()
    {
        blender.SetFocus(5f, 1f);
        disclaimerPanel.StartDismiss();
        //depthOfField.active = false;
    }

    public void Activate()
    {
        uiObject.SetActive(true);

    }

    private void EntranceClicked()
    {
        blender.ActivateDepthOfField(true);
        focusDistanceFinder.enabled = false;
        disclaimerPanel.gameObject.SetActive(true);
        blender.SetFocus(0.4f, 1f);
    }

    public void Deactivate()
    {
        uiObject.SetActive(false);
    }


}