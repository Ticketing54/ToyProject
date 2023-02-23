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
            // 수정할 것
            Current = unLogin;
        }
        else
        {
            Destroy(uiManager);
        }
        // 추후에 게임메니저에서 할 것!
        AuthManager.Instance.Init();
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
    ///  GameState 에따른 UINavigation 반환
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
    /// 게임 씬 상태에 따른 UINavigation 변화
    /// </summary>
    /// <param name="GameState"></param>
    public void ChangeUINavgation(GameState _state)
    {
        Current.gameObject.SetActive(false);
        Current = GetUINavigation(_state);
        Current.gameObject.SetActive(true);
        Current.gameObject.transform.position = Vector3.zero;
    }    
    public void CurrentPop()
    {
        if(Current == null)
        {
            Debug.LogError("Current UINavigation is NULL");
            return;
        }
        Current.Pop();
    }
    #region DontClick && ErrorMessage    
    [SerializeField]
    DontClick dontClick;    
    /// <summary>
    /// 화면 클릭 방지용(On)
    /// </summary>
    public void OnDontClick() { dontClick.gameObject.SetActive(true); }
    /// <summary>
    /// 화면 클릭 반지용(Off)
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


}
