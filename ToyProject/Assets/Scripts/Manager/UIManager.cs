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
            current = unLogin;
        }
        else
        {
            Destroy(uiManager);
        }

        AuthManager.Instance.Init();
        OpenDontClick += () => { dontClick.gameObject.SetActive(true); };
        CloseDontClick += () => { dontClick.gameObject.SetActive(false); };        
    }
    private void Start()
    {
        
    }
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
    public Action OpenDontClick { get; private set; }
    public Action CloseDontClick { get; private set; }
    public void OnErrorMessage(string _msg)
    {
        OpenDontClick();
        dontClick.SetErrorMessage(_msg);
    }

    
    public void CurrentPop()
    {
       //
    }
    #endregion


}
