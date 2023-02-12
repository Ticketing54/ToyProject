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
    // current present
    private UINavigation current;

    // UINavigation in GameSate 
    [SerializeField]
    UINavigation login;
    [SerializeField]
    UINavigation lobby;
    [SerializeField]
    UINavigation playing;

    UINavigation GetUINavigation(GameState _state)
    {
        switch (_state)
        {
            case GameState.Login:
                return login;
            case GameState.Lobby:
                return lobby;
            case GameState.Playing:
                return playing;
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

    #region ErrorMessage    
    [SerializeField]
    ErrorMessage errorMessage;
    public void OnErrorMessage(string _msg)
    {
        errorMessage.gameObject.SetActive(true);
        errorMessage.transform.SetParent(current.transform);
        errorMessage.transform.localPosition = Vector3.zero;
        errorMessage.SetErrorMessage(_msg);
    }
    #endregion
    public void OnDontTouch()
    {
        errorMessage.gameObject.SetActive(true);
        errorMessage.transform.SetParent(current.transform);
        errorMessage.transform.localPosition = Vector3.zero;
    }    
   
    

}
