using System.Collections;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine;
using System;
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
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
        yield return null;                
        switch (_state)
        {
            case GameState.Login:
                {
                    yield return SceneManager.LoadSceneAsync("LoginScene");
                    StartCoroutine(CoLogin());
                }
                break;
            case GameState.Lobby:
                {
                    yield return SceneManager.LoadSceneAsync("LobbyScene");
                    
                }
                break;
            case GameState.Playing:
                {
                    yield return SceneManager.LoadSceneAsync("PlayScene");
                    
                }
                break;
            default:
                {
                    Debug.LogError("GaemeState Error (Enum Default)");
                }
                break;

        }
        yield return new WaitForSeconds(5f);
        State = _state;
        UIManager.uiManager.CloseLoadingUI();
    }
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

    #region Patch    

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
        while(_handle.IsValid())
        {   
            DownloadStatus downstatus = _handle.GetDownloadStatus();
            UpdatePatchUI(downstatus.DownloadedBytes, downstatus.TotalBytes, downstatus.Percent);
            yield return null;
        }
    }
    #endregion

}
