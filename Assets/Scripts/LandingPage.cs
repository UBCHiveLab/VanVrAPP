using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Controller;
using UnityEngine;
using UnityEngine.UI;

public class LandingPage: MonoBehaviour, IPage
{
  public StateController stateController;
  public Button startButton;
	public GameObject doorL;
	public GameObject doorR;
	public GameObject doors;
	public GameObject mainCamera;

	void Start()
	{
		Button btn = startButton.GetComponent<Button>();
		btn.onClick.AddListener(StartSession);
	}

	void StartSession() 
  {
    doors.GetComponent<Animator>().SetTrigger("Start");
    mainCamera.GetComponent<Animator>().SetTrigger("Start");
    stateController.mode = ViewMode.TRAY;
  }

  public void Activate() 
  {
    startButton.gameObject.SetActive(true);
  }

  public void Deactivate()
  {
    startButton.gameObject.SetActive(false);
	}

}

