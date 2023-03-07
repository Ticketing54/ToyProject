using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class LobbyManager : MonoBehaviourPunCallbacks
{
    private void Start()
    {
        
    }
<<<<<<< HEAD
    public void ConnectMainSever()
    {
        if(AuthManager.Instance.User == null)
        {
            Debug.LogError("AuthManager User is Null");
            GameManager.Instance.ChangeState(GameState.Login);
        }

    }
  public void test()
    {
=======
>>>>>>> 40e6cbfd2d814b74c0b01c2e380baac49d7a44b0

    public override void OnConnectedToMaster()
    {
        
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        
    }

}
