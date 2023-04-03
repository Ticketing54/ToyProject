using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{   
    public static UIManager uiManager;
    private void Awake()
    {
        if (uiManager == null)
        {
            uiManager = this;
            DontDestroyOnLoad(this.gameObject);                        
        }
        else
        {
            Destroy(uiManager);
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
    /// 게임 씬 상태에 따른 UINavigation 변화
    /// </summary>
    /// <param name="GameState"></param>
    public void ChangeUINavgation(GameState _state)
    {
        if(Current != null)
        {
            Current.gameObject.SetActive(false);
        }
        Current = GetUINavigation(_state);
        Current.gameObject.SetActive(true);
        Current.gameObject.transform.localPosition= Vector3.zero;
        Current.RootShow();
    }
    /// <summary>
    ///  GameState 로 UINavigation 반환
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
        if(Current == null)
        {
            Debug.LogError("Current UINavigation is NULL");
            return;
        }
        Current.Pop();
    }

    #region LoadingUI
    [SerializeField]
    GameObject loadingUI;
    public void OpenLoadingUI() { loadingUI.gameObject.SetActive(true); }
    public void CloseLoadingUI() { loadingUI.gameObject.SetActive(false); }
    #endregion

    #region DontClick && ErrorMessage    
    [SerializeField]
    DontClick dontClick;    
    /// <summary>
    /// 화면 클릭 방지용(On)
    /// </summary>
    public void OnDontClick() { dontClick.gameObject.SetActive(true); }
    /// <summary>
    /// 화면 클릭 방지용(Off)
    /// </summary>
    public void OFFDontClick() { dontClick.gameObject.SetActive(false); }
    /// <summary>
    /// 오류 메세지 출력
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
    public Action<List<UserInfo>> ARoomUpdate { get; set; }

    #endregion
}
