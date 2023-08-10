using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using Photon.Pun;
public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(Instance);
        }
    }

    // Current UINavigation
    public UINavigation Current { get; private set; }

    // UINavigation in GameSate     
    [SerializeField]
    UINavigation unLogin;
    [SerializeField]
    UINavigation unLobby;
    [SerializeField]
    UINavigation unplaying;


    /// <summary>
    /// ���� �� ���¿� ���� UINavigation ��ȭ
    /// </summary>
    /// <param name="GameState"></param>
    public void ChangeUINavgation(GameState _state)
    {
        if (Current != null)
        {
            Current.gameObject.SetActive(false);
        }
        Current = GetUINavigation(_state);
        Current.gameObject.SetActive(true);
        Current.gameObject.transform.localPosition = Vector3.zero;
        Current.RootShow();
    }
    /// <summary>
    ///  GameState �� UINavigation ��ȯ
    /// </summary>
    /// <param name="GameState"></param>
    /// <returns></returns>
    UINavigation GetUINavigation(GameState _state)
    {
        switch (_state)
        {
            case GameState.Login:
                return unLogin;
            case GameState.Lobby:
                return unLobby;
            case GameState.Playing:
                return unplaying;
            default:
                Debug.LogError("Wrong Enum GameState");
                return null;
        }
    }

    /// <summary>
    /// Back Button
    /// </summary>
    public void CurrentPop()
    {
        if (Current == null)
        {
            Debug.LogError("Current UINavigation is NULL");
            return;
        }
        Current.Pop();
    }

    #region LoadingUI & PatchUI
    [SerializeField]
    LoadingUI loadingUI;
    [SerializeField]
    PatchUI patchUI;
    public LoadingUI LoadingUIInstance { get => loadingUI; set => loadingUI = value; }
    public PatchUI PatchUIInstacne { get => patchUI; set => patchUI = value; }
    #endregion

    #region DontClick && ErrorMessage    
    [SerializeField]
    DontClick dontClick;    
    /// <summary>
    /// ȭ�� Ŭ�� ������(On)
    /// </summary>
    public void OnDontClick() { dontClick.gameObject.SetActive(true); }
    /// <summary>
    /// ȭ�� Ŭ�� ������(Off)
    /// </summary>
    public void OFFDontClick() { dontClick.gameObject.SetActive(false); }
    /// <summary>
    /// ���� �޼��� ���
    /// </summary>
    /// <param name="Messege"></param>
    public void OnErrorMessage(string _msg)
    {
        OnDontClick();
        dontClick.SetErrorMessage(_msg);
    }
    #endregion


    #region Login
    /// <summary>
    /// NickNameSettingUI On
    /// </summary>
    public Action AOpenNickNameUI { get; set; }

    #endregion

    #region Lobby
    /// <summary>
    /// Open UIView Room
    /// </summary>
    public Action AOpenRoom { get; set; }
    /// <summary>
    /// LobbyMainSetting
    /// </summary>
    public Action<UserInfo> ALobbyPlayerSetting { get; set; }
    /// <summary>
    /// Push Marking
    /// </summary>
    public Action AMarkingFriendButton { get; set; }
    /// <summary>
    ///  Check FriendRequestMessage
    /// </summary>
    public Action<UserInfo> ACheckFriendRequests { get; set; }
    /// <summary>
    /// FriendListClear(UI)
    /// </summary>
    public Action AFriendListClear { get; set; }
    /// <summary>
    /// UIFriendList Add Friend
    /// </summary>
    public Action<UserInfo> AFriendAdd { get; set; }
    /// <summary>
    /// InvitationMessageOpen (Userinfo,RoomName)
    /// </summary>
    public Action<UserInfo, string> AOpenInvitationMessage { get; set; }
    /// <summary>
    /// UI RoomStateUpdate
    /// </summary>
    public Action<Queue<UserInfo>> ARoomUpdate { get; set; }

    public Action AOpenCounter { get; set; }
    public Action<int> ACountNumber { get; set; }
    #endregion
    #region Play
    public Action<float> ATimer { get; set; }
    #endregion

}