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
            login += (ID, PW)  =>{ AuthManager.Instance.Login(ID, PW, OnErrorMessage); };
            regist += (ID, PW) => { AuthManager.Instance.Regist(ID, PW, OnErrorMessage); };
            current = unLogin;
        }
        else
        {
            Destroy(uiManager);
        }
    }
    private void Start()
    {
        AuthManager.Instance.Init();
    }

    public Action<string, string> Login { get => login; }
    public Action<string,string> Regist { get => regist; }
    Action<string, string> login;
    Action<string, string> regist;

    // current present
    private UINavigation current;

    // UINavigation in GameSate 
    [SerializeField]
    UINavigation unLogin;
    [SerializeField]
    UINavigation unLobby;
    [SerializeField]
    UINavigation unplaying;

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
    public void ChangeUINavgation(GameState _state)
    {
        current.gameObject.SetActive(false);
        current = GetUINavigation(_state);
        current.gameObject.SetActive(true);
        current.gameObject.transform.position = Vector3.zero;
    }

    #region DontClick && ErrorMessage    
    [SerializeField]
    DontClick dontClick;
    public void OnErrorMessage(string _msg)
    {
        OnDontTouch();
        PostData test = new PostData();
        dontClick.SetErrorMessage(test);
        
    }

    public void OnDontTouch()
    {
        dontClick.gameObject.SetActive(true);
        dontClick.transform.SetParent(current.transform);
        dontClick.transform.localPosition = Vector3.zero;
    }
    public void OffDontTouch()
    {
        dontClick.transform.SetParent(null);
        dontClick.gameObject.SetActive(false);        
    }
    #endregion


}
