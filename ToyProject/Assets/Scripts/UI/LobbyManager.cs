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

    }

}
