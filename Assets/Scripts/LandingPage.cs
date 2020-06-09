using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LandingPage: MonoBehaviour
{
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

	void StartSession() {
		doors.GetComponent<Animator>().SetTrigger("Start");
		mainCamera.GetComponent<Animator>().SetTrigger("Start");
		startButton.gameObject.SetActive(false);
;
	}

}

