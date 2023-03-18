using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class LobbyManager : MonoBehaviourPunCallbacks
{
    private readonly string gameVersion = "1";

    private void Awake()
    {   
        GameManager.Instance.ConnectMainServer += ConnectUsingSetting;
    }


    void ConnectUsingSetting()
    {
        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.AuthValues.UserId = AuthManager.Instance.User.UserId;        
        PhotonNetwork.ConnectUsingSettings();
        UIManager.uiManager.OnDontClick();
    }
    public override void OnConnectedToMaster()
    {
        UIManager.uiManager.OFFDontClick();

    }
    public override void OnDisconnected(DisconnectCause cause)
    {

        PhotonNetwork.ConnectUsingSettings();
    }

    
}
