using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class LobbyManager : MonoBehaviourPunCallbacks
{
    private static LobbyManager instance;
    public static LobbyManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new LobbyManager();                
            }
            return instance;
        }
        
    }

    public override void OnConnectedToMaster()
    {
        
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        
    }

}
