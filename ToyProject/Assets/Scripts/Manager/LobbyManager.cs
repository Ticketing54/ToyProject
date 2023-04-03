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
    public void CreatRoom()
    {
        if (PhotonNetwork.InRoom)
            return;

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;
        roomOptions.IsOpen = true;
        PhotonNetwork.CreateRoom(null, roomOptions);
    }
    public void JoinRoom(string _roomName) { PhotonNetwork.JoinRoom(_roomName); }
    public override void OnJoinedRoom()
    {
        AuthManager.Instance.JoinRoom(PhotonNetwork.CurrentRoom.Name);
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        UIManager.uiManager.OnErrorMessage(message);
    }

    public void SendInvitationMessage(string _targetUID)
    {
        AuthManager.Instance.SendInvitationMessage(_targetUID,PhotonNetwork.CurrentRoom.Name);
    }

    public override void OnCreatedRoom()
    {
        AuthManager.Instance.CreateRoom(PhotonNetwork.CurrentRoom.Name);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        List<string> playersUID = new List<string>();
        playersUID.Add(PhotonNetwork.MasterClient.UserId);
        foreach(Player player in PhotonNetwork.PlayerList)
        {
            if(player.UserId != playersUID[0])
            {
                playersUID.Add(player.UserId);
            }
        }
        AuthManager.Instance.UpdateRoom(playersUID);
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        List<string> playersUID = new List<string>();
        playersUID.Add(PhotonNetwork.MasterClient.UserId);
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.UserId != playersUID[0])
            {
                playersUID.Add(player.UserId);
            }
        }
        AuthManager.Instance.UpdateRoom(playersUID);
    }
    public void LeaveRoom()
    {
        if(PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            AuthManager.Instance.DestroyRoom(PhotonNetwork.CurrentRoom.Name);
        }
        PhotonNetwork.LeaveRoom();
    }

    public void ConnectSetting()
    {
        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.AuthValues = new AuthenticationValues(AuthManager.Instance.User.UserId);
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.LocalPlayer.CustomProperties["UID"] = AuthManager.Instance.User.UserId;
    }
}
