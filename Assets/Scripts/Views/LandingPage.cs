using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Controller;
using UnityEngine;
using UnityEngine.UI;

public class LandingPage : MonoBehaviour, IPage
{
    public StateController stateController;
    public GameObject uiObject;
    public Button startButton;
    public GameObject doorL;
    public GameObject doorR;
    public GameObject mainCamera;
    public Animator mainCameraAnimator;
    public AudioSource startAudio;

    void Start()
    {
        Button btn = startButton.GetComponent<Button>();
        btn.onClick.AddListener(StartSession);
        mainCameraAnimator = Camera.main.GetComponent<Animator>();
    }

    void StartSession()
    {
        startAudio.GetComponent<AudioSource>().Play(0);
        doorL.GetComponent<Animator>().SetTrigger("Start");
        doorR.GetComponent<Animator>().SetTrigger("Start");
        mainCameraAnimator.SetTrigger("Start");
        stateController.mode = ViewMode.TRAY;
    }

    public void Activate()
    {
        uiObject.SetActive(true);
    }

    public void Deactivate()
    {
        uiObject.SetActive(false);
    }
}