using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class LobbyManager : MonoBehaviourPunCallbacks
{
    private readonly string gameVersion = "1";
    private static LobbyManager instance;
    public static LobbyManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<LobbyManager>();
                if(instance == null)
                {
                    GameObject obj = new GameObject("LobbyManager");                    
                    instance =  obj.AddComponent<LobbyManager>();
                    DontDestroyOnLoad(instance.gameObject);
                }
            }

            return instance;
        }        
    }    
    public void ConnectUsingSetting()
    {
        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.AuthValues = new AuthenticationValues(AuthManager.Instance.User.UserId);
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