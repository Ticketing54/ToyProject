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
    DontClick dontClickscrip;
    [SerializeField]
    public GameObject dontClick;
    public void OnDontClick() { dontClick.SetActive(true); }
    public void OFFDontClick() { dontClick.SetActive(false); }
    
    public void OnErrorMessage(string _msg)
    {
        if(dontClick.gameObject.activeSelf == true)
        {
            Debug.Log("Dd");
        }
        dontClick.SetActive(true);
        dontClickscrip.SetErrorMessage(_msg);
    }

    
    public void CurrentPop()
    {
       //
    }
    #endregion


}
