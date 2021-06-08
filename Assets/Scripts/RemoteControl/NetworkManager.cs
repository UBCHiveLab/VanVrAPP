using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_InputField codeInputField = null;
    [SerializeField] public Button connectButton;
    [SerializeField] public Button startButton;
    [SerializeField] public TextMeshProUGUI log;
    public GameObject canvas;
    public AnalysisPage analysisPage;

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    // Update is called once per frame
    void Update()
    {
        
            
        
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("Network is connected");
        
    }

    public void CreateRoomCode()
    {
        if (codeInputField.text != null)
        {
            PhotonNetwork.CreateRoom(codeInputField.text, new Photon.Realtime.RoomOptions() { MaxPlayers = 2 }, default);
            Debug.Log("created a room");
        }
    }

    public void JoinRoom()
    {
        if (codeInputField.text != null)
        {
            PhotonNetwork.JoinRoom(codeInputField.text);
            Debug.Log("Joined a room");
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Client successfully joined a room");

        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;

        if(playerCount != 2)
        {
            Debug.Log("connection is not ready");
            log.text = playerCount + " devices in this room";
        }
        else
        {
            Debug.Log("ready to start");
            log.text = playerCount + " devices in this room";
            canvas.gameObject.SetActive(false);
            //analysisPage.MatchReferenceRotation();
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        
    }
}
