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
        UIManager.uiManager.OpenLoadingUI();
        StartCoroutine(CoPatchCheck());
    }
    public GameState State { get; private set; }
    /// <summary>
    /// 게임 상태 변경 (Enum타입)
    /// </summary>
    /// <param name="GameState"></param>    
    public void ChangeState(GameState _state) { StartCoroutine(CoChangeState(_state)); }    
    IEnumerator CoChangeState(GameState _state)
    {
        // 로딩 UI 열기
        UIManager.uiManager.OpenLoadingUI();
        State = _state;
        yield return new WaitForSeconds(1f);                

        switch (_state)
        {
            case GameState.Login:
                {   
                    if(SceneManager.GetActiveScene().name != "LoginScene")
                    {
                        yield return SceneManager.LoadSceneAsync("LoginScene");
                    }
                    yield return CoPatchCheck();
                }
                yield break;
            case GameState.Lobby:
                {
                    if(AuthManager.Instance.User == null)
                    {
                        ChangeState(GameState.Login);
                        yield break;
                    }
                    yield return SceneManager.LoadSceneAsync("LobbyScene");
                    LobbyManager.Instance.ConnectSetting();
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
        UIManager.uiManager.CloseLoadingUI();
    }

    #region Patch
    /// <summary>
    /// PatchUI SizeSetting
    /// </summary>
    public Action<long> SettingPatch;
    /// <summary>
    /// OpenPatchUI
    /// </summary>
    public Action OpenPatchUI;
    /// <summary>
    /// ClosePatchUI
    /// </summary>
    public Action ClosePatchUI;
    /// <summary>
    /// UpdateCurent UpdateSize UpdateProgress
    /// </summary>
    public Action<long, long, float> UpdatePatchUI;
    IEnumerator CoPatchCheck()
    {
        yield return new WaitForSeconds(2f);
        OpenPatchUI();
        yield return AuthManager.Instance.Init();
        AsyncOperationHandle<long> sizeCheck = Addressables.GetDownloadSizeAsync("Patch");
        yield return sizeCheck;
        if (sizeCheck.Result == 0)
        {
            UIManager.uiManager.ChangeUINavgation(GameState.Login);
            ClosePatchUI();
        }
        else
        {
            SettingPatch(sizeCheck.Result);
        }
        // 로딩화면 닫기
        UIManager.uiManager.CloseLoadingUI();
    }
    // UIPatchButton에 등록할 함수
    public void DownloadPatch()
    {
        StartCoroutine(CoPatchDownLoad());
    }

    IEnumerator CoPatchDownLoad()
    {
        AsyncOperationHandle patch = Addressables.DownloadDependenciesAsync("Patch", true);
        yield return StartCoroutine(UpdateUI(patch));
        UIManager.uiManager.ChangeUINavgation(GameState.Login);
        ClosePatchUI();
    }
    IEnumerator UpdateUI(AsyncOperationHandle _handle)
    {
        while (_handle.IsValid())
        {   
            DownloadStatus downstatus = _handle.GetDownloadStatus();
            UpdatePatchUI(downstatus.DownloadedBytes, downstatus.TotalBytes, downstatus.Percent);
            yield return null;
        }
    }
    #endregion

}
