using Assets.Scripts.State;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

/**
 * Defines logic and composes components for Landing mode.
 */

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
    private DepthOfField depthOfField;


    void Start()
    {
      //  mainCameraAnimator = Camera.main.GetComponent<Animator>();

        disclaimerPanel.gameObject.SetActive(false);
        disclaimerPanel.enterAction = StartSession;

        if (depthOfField == null) {
            volume.profile.TryGetSettings(out depthOfField);
        }

        focusDistanceFinder.enabled = false;
    //    disclaimerPanel.gameObject.SetActive(true);
        depthOfField.active = true;
        depthOfField.focusDistance.value = 1f;
    }

    void StartSession()
    {
        startAudio.GetComponent<AudioSource>().Play(0);
        doorL.GetComponent<Animator>().SetTrigger("Start");
        doorR.GetComponent<Animator>().SetTrigger("Start");
    //    mainCameraAnimator.SetTrigger("Start");
        DismissDisclaimer();
    }

    public void DismissDisclaimer()
    {
        depthOfField.active = false;
        disclaimerPanel.gameObject.SetActive(false);
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
