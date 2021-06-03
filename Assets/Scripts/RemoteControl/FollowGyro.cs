using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FollowGyro : MonoBehaviourPun
{

    //This script is to connect the object rotation to user's phone

    [Header("Tweaks")]
    [SerializeField] private Quaternion baseRotation = new Quaternion(0, 0, 1, 0);

    // Start is called before the first frame update
    private void Start()
    {
        GyroManager.Instance.EnableGyro();
    }

    // Update is called once per frame
    private void Update()
    {
        if (photonView.IsMine&& PhotonNetwork.IsConnected)
        {
            transform.localRotation = GyroManager.Instance.GetGyroRotation() * baseRotation;

        }
    }
}
