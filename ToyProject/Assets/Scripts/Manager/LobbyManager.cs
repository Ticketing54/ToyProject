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

    private void Awake()
    {
        Application.quitting +=
            () =>
            {
                if(PhotonNetwork.IsConnected == true)
                {
                    PhotonNetwork.Disconnect();
                }
            };
    }
    /// <summary>
    /// ConnectSetting
    /// </summary>
    public void ConnectSetting()
    {
        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.AuthValues = new AuthenticationValues(AuthManager.Instance.User.UserId);
        PhotonNetwork.LocalPlayer.CustomProperties.Add("UID", AuthManager.Instance.User.UserId);
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.JoinLobby();
    }
    /// <summary>
    /// SendInviteationMessage
    /// </summary>
    /// <param name="TargetUID"></param>
    public void SendInvitationMessage(string _targetUID)
    {
        AuthManager.Instance.SendInvitationMessage(_targetUID, PhotonNetwork.CurrentRoom.Name);
    }
    /// <summary>
    /// UpdateRoom
    /// </summary>
    void UpdateRoom()
    {
        List<string> playersUID = new List<string>();
        string master = PhotonNetwork.CurrentRoom.CustomProperties["Master"].ToString();
        playersUID.Add(master);
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            string userUID = player.CustomProperties["UID"].ToString();
            if (userUID == master)
                continue;
            playersUID.Add(userUID);
        }
        AuthManager.Instance.UpdateRoom(PhotonNetwork.CurrentRoom.Name, playersUID);
    }
    /// <summary>
    /// CreateRoom
    /// </summary>
    public void CreatRoom()
    {
        if (PhotonNetwork.InRoom)
            return;
        UIManager.Instance.LoadingUIInstance.OpenLoadingUI(false);     // 로딩 UI 활성화

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;
        roomOptions.IsOpen = true;
        ExitGames.Client.Photon.Hashtable masterinfo = new ExitGames.Client.Photon.Hashtable();
        masterinfo.Add("Master", AuthManager.Instance.User.UserId);
        roomOptions.CustomRoomProperties = masterinfo;
        PhotonNetwork.CreateRoom(null, roomOptions);
    }
    public override void OnCreatedRoom()
    {
        UIManager.Instance.AOpenRoom();
        UpdateRoom();
        UIManager.Instance.LoadingUIInstance.CloseLoadingUI();
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        UIManager.Instance.OnErrorMessage("방생성에 실패했습니다.");
    }
    /// <summary>
    /// JoinRoom
    /// </summary>
    /// <param name="_roomName"></param>
    public void JoinRoom(string _roomName) 
    {
        UIManager.Instance.LoadingUIInstance.OpenLoadingUI(false);
        PhotonNetwork.JoinRoom(_roomName);
    }
    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            return;
        UIManager.Instance.AOpenRoom();
        UpdateRoom();
        UIManager.Instance.LoadingUIInstance.CloseLoadingUI();
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        UIManager.Instance.OnErrorMessage(message);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdateRoom();
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdateRoom();
    }
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if(newMasterClient.CustomProperties["UID"].ToString() == PhotonNetwork.AuthValues.UserId)
        {
            ExitGames.Client.Photon.Hashtable master = PhotonNetwork.CurrentRoom.CustomProperties;
            master["Master"] = PhotonNetwork.LocalPlayer.CustomProperties["UID"].ToString();
            PhotonNetwork.CurrentRoom.SetCustomProperties(master);
        }
    }
    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        UpdateRoom();
    }
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.JoinLobby();
    }
    public void StartGame()
    {
        // 카운트 제작 할 것!
        UIManager.Instance.LoadingUIInstance.OpenLoadingUI();
        PhotonNetwork.LoadLevel("PlayScene");
        StartCoroutine(CoStartGame());
    }
    IEnumerator CoStartGame()
    {
        bool isLoadedScene = false;
        while(!isLoadedScene)
        {
            if(PhotonNetwork.LevelLoadingProgress >= 1)
            {
                isLoadedScene = true;
            }
            yield return null;
        }
        GameManager.Instance.ChangeState(GameState.Playing);
    }
}
