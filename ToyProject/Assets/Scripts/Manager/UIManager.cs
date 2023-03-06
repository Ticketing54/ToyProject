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
        Current = unLogin;
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
        Current.gameObject.SetActive(false);
        Current = GetUINavigation(_state);
        Current.gameObject.SetActive(true);
        Current.gameObject.transform.localPosition= Vector3.zero;
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

    #region Login
    
    public Action OpenLoginUI
    {
        get
        {
            if(Current != unLogin)
            {
                Current.gameObject.SetActive(false);
                Current = unLogin;
            }
            return openLoginUi;
        }
        set
        {
            if (value == null)
            {
                Debug.LogError("value is Null(OpenLoginUI)");
            }
            else
                openLoginUi = value;
        }


    }
    /// <summary>
    /// PatchSize 
    /// </summary>
    public Action<long> OpenPatchUI
    {
        get
        {
            if (Current != unLogin)
            {
                Current.gameObject.SetActive(false);
                Current = unLogin;
            }
            return openPatchUI;
        }
        set
        {
            if (value == null)
            {
                Debug.LogError("value is Null(OpenPatchUI");
            }
            else
            {
                openPatchUI = value;
            }
        }

    }
    private Action openLoginUi;
    private Action<long> openPatchUI;
    #endregion
    #region Lobby


    #endregion




    #region LoadingUI
    [SerializeField]
    LoadingUI loadingUI;
    public void OpenLoadingUI()
    {
        loadingUI.gameObject.SetActive(true);
        loadingUI.Show();
    }
    /// <summary>
    /// 종료 시 자동으로 SetActive False;
    /// </summary>
    public void CloseLoadingUI() { loadingUI.Hide(); }
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
}
