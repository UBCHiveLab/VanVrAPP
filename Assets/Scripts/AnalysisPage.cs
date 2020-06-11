using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Controller;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class AnalysisPage : MonoBehaviour
{
    public StateController stateController;
    [Header("Control Assistant")]
    public GameObject controlAssistantGO;
    public Button controllerAssistant;
    public Button controlButtonUp;
    public Button controlButtonDown;
    public Button controlButtonLeft;
    public Button controlButtonRight;
    public Button zoomIn;
    public Button zoomOut;
	public GameObject mainCamera;
    public Transform specimenInTray;

    [Header("Reset Camera Position")]
    public Button resetButton;

    private float xPos;
    private float yPos;
    private float zPos;

	void Start()
	{
        // Control Assistant Buttons

        Button controlAssistant = controllerAssistant.GetComponent<Button>();
		Button up = controlButtonUp.GetComponent<Button>();
        Button down = controlButtonDown.GetComponent<Button>();
        Button left = controlButtonLeft.GetComponent<Button>();
        Button right = controlButtonRight.GetComponent<Button>();
        Button zoomInside = zoomIn.GetComponent<Button>();
        Button zoomOutside = zoomOut.GetComponent<Button>();


		up.onClick.AddListener(MoveUp);
        down.onClick.AddListener(MoveDown);
        left.onClick.AddListener(MoveLeft);
        right.onClick.AddListener(MoveRight);
        zoomInside.onClick.AddListener(ZoomIn);
        zoomOutside.onClick.AddListener(ZoomOut);
        controlAssistant.onClick.AddListener(ToggleController);


        // Reset Specimen Button

        Button resetCameraPosition = resetButton.GetComponent<Button>();
        resetCameraPosition.onClick.AddListener(ResetCameraPosition);

    }

    void Update()
    {
        xPos = mainCamera.transform.position.x;
        yPos = mainCamera.transform.position.y;
        zPos = mainCamera.transform.position.z;
        mainCamera.transform.LookAt(specimenInTray);

        // print(xPos + "  " + yPos + "  " + zPos);

    }

    // CONTROL ASSISTANT BUTTON METHODS

    void ToggleController()
    {
        print("Toggle Controller On/Off");
        controlAssistantGO.SetActive(!controlAssistantGO.activeInHierarchy);

        // Change MS camera setting from Orbital to Look At Player when turning on control assistant.

    }

    void MoveUp() 
    {
        print("UP");
        yPos += 1f;
        mainCamera.transform.position = new Vector3(xPos, yPos, zPos);
    }

    void MoveDown() 
    {
        print("DOWN");
        yPos -= 1f;
        mainCamera.transform.position = new Vector3(xPos, yPos, zPos);
    }

    void MoveLeft() 
    {
        print("LEFT");
        xPos -= 1f;
        mainCamera.transform.position = new Vector3(xPos, yPos, zPos);
    }

    void MoveRight() 
    {
        print("RIGHT");
        xPos += 1f;
        mainCamera.transform.position = new Vector3(xPos, yPos, zPos);
    }

    void ZoomIn() 
    {
        print("Zoom In");
        zPos += 1f;
        mainCamera.transform.position = new Vector3(xPos, yPos, zPos);
    }

    void ZoomOut() 
    {
        print("Zoom Out");
        zPos -= 1f;
        mainCamera.transform.position = new Vector3(xPos, yPos, zPos);
    }

    // RESET BUTTON METHOD

    void ResetCameraPosition()
    {
        print("Reset");
        mainCamera.transform.position = new Vector3(0, 4, -6);
        
        mainCamera.transform.rotation = Quaternion.Euler(10, 0, 0);
    }
  
}
