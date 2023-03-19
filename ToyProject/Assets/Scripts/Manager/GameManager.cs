using System.Collections;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine;
using System;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }        
    }
    private void Start()
    {
        State = GameState.Login;
        StartCoroutine(CoLogin());        
    }
    public GameState State { get; private set; }
    /// <summary>
    /// 게임 상태 변경 (Enum타입)
    /// </summary>
    /// <param name="GameState"></param>    
    public void ChangeState(GameState _state) { StartCoroutine(CoChangeState(_state)); }    
    IEnumerator CoChangeState(GameState _state)
    {
        UIManager.uiManager.OpenLoadingUI();

        yield return new WaitForSeconds(1f);                

        switch (_state)
        {
            case GameState.Login:
                {   
                    // Login Scene에서는 RootView가 Patch일수도 Login일수도 있어서 
                    // ChangeUIVavgation을 사용하지 않았다.
                    yield return SceneManager.LoadSceneAsync("LoginScene");
                    yield return StartCoroutine(CoLogin());
                    UIManager.uiManager.CloseLoadingUI();
                }
                break;
            case GameState.Lobby:
                {
                    if(AuthManager.Instance.User == null)
                    {
                        ChangeState(GameState.Login);
                        yield break;
                    }
                    yield return SceneManager.LoadSceneAsync("LobbyScene");
                    LobbyManager.Instance.ConnectUsingSetting();
                    UIManager.uiManager.ChangeUINavgation(_state);
                }
                break;
            case GameState.Playing:
                {
                    yield return SceneManager.LoadSceneAsync("PlayScene");
                    UIManager.uiManager.ChangeUINavgation(_state);
                }
                break;
            default:
                {
                    Debug.LogError("GaemeState Error (Enum Default)");                    
                }
                break;

        }
        
        State = _state;        
    }

    #region Login    
    IEnumerator CoLogin()
    {
        yield return AuthManager.Instance.Init();
        AsyncOperationHandle<long> sizeCheck = Addressables.GetDownloadSizeAsync("Patch");
        yield return sizeCheck;
        if (sizeCheck.Result == 0)
        {
            UIManager.uiManager.OpenLoginUI();
        }
        else
        {
            UIManager.uiManager.OpenPatchUI(sizeCheck.Result);
        }
    }

    /// <summary>
    /// UpdateCurent UpdateSize UpdateProgress
    /// </summary>
    public Action<long, long, float> UpdatePatchUI
    {
        get => updatePatchUI;
        set
        {
            if (value == null)
            {
                Debug.LogError("value is null(GameManager->UpdatePatchUI)");
            }
            else
                updatePatchUI = value;
        }
    }
    private Action<long, long, float> updatePatchUI;    
    // UIPatchButton에 등록할 함수
    public void DownloadPatch()
    {
        StartCoroutine(CoPatchDownLoad());
    }
   
    IEnumerator CoPatchDownLoad()
    {   
        AsyncOperationHandle patch = Addressables.DownloadDependenciesAsync("Patch", true);        
        yield return StartCoroutine(UpdateUI(patch));
        UIManager.uiManager.OpenLoginUI();
    }
    IEnumerator UpdateUI(AsyncOperationHandle _handle)
    {
        while(!_handle.IsValid())
        {
            yield return null;
            DownloadStatus downstatus = _handle.GetDownloadStatus();
            UpdatePatchUI(downstatus.DownloadedBytes, downstatus.TotalBytes, downstatus.Percent);            
        }
    }
    #endregion
}
