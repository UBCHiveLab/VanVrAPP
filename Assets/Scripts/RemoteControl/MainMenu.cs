using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject findDevicePanel = null;
    [SerializeField] private GameObject waitingStatusPanel = null;
    [SerializeField] private TextMeshProUGUI waitingStatusText = null;

    [SerializeField] private Button connectButton;
    [SerializeField] private Button startButton;
    [SerializeField] private InputField roomInput;


    private bool isConnecting = false;
    private const int MaxPlayerPerRoom = 2;
    private int roomNum;

    public void FindPairDevice()
    {
        isConnecting = true;

        findDevicePanel.SetActive(false);
        waitingStatusPanel.SetActive(true);

        waitingStatusText.text = "Searching...";


        if (PhotonNetwork.IsConnected)
        {
            //pair the device
        }else
        {
            //show error
        }
    }
}
