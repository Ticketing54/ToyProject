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
        UIManager.Instance.LoadingUIInstance.OpenLoadingUI();
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
        UIManager.Instance.LoadingUIInstance.OpenLoadingUI();
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
                    UIManager.Instance.ChangeUINavgation(_state);
                }
                break;
            case GameState.Playing:
                {
                    yield return StartCoroutine(GameSetting());
                    yield break;
                }
            default:
                {
                    Debug.LogError("GaemeState Error (Enum Default)");                    
                }
                break;
        }
        UIManager.Instance.LoadingUIInstance.CloseLoadingUI();
    }

    IEnumerator GameSetting()
    {
        
        yield return null;
        UIManager.Instance.LoadingUIInstance.OpenLoadingUI(true);
        UIManager.Instance.LoadingUIInstance.ProgressSetting(3, () =>
         {
             UIManager.Instance.LoadingUIInstance.CloseLoadingUI();
             UIManager.Instance.ChangeUINavgation(GameState.Playing);
         });
        yield return AuthManager.Instance.SettingTable();          // Table
        UIManager.Instance.LoadingUIInstance.CurrentStep++;
        yield return ResourceManager.Instance.PrefabSetting();     // Prefab 
        UIManager.Instance.LoadingUIInstance.CurrentStep++;
        // MapSetting
        
        // OpeningCarmeraSetting

        // GameStart
    }
    #region Patch
    IEnumerator CoPatchCheck()
    {   
        yield return new WaitForSeconds(2f);
        UIManager.Instance.PatchUIInstacne.OpenPatchUI();
        yield return AuthManager.Instance.Init();
        UIManager.Instance.LoadingUIInstance.CloseLoadingUI();
        AsyncOperationHandle<long> sizeCheck = Addressables.GetDownloadSizeAsync("Patch");
        yield return sizeCheck;
        if (sizeCheck.Result == 0)
        {
            UIManager.Instance.ChangeUINavgation(GameState.Login);
        }
        else
        {
            UIManager.Instance.PatchUIInstacne.SetPatchUI(sizeCheck.Result);
        }
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
        UIManager.Instance.ChangeUINavgation(GameState.Login);
        UIManager.Instance.PatchUIInstacne.ClosePatchUI();
    }
    IEnumerator UpdateUI(AsyncOperationHandle _handle)
    {
        while (_handle.IsValid())
        {   
            DownloadStatus downstatus = _handle.GetDownloadStatus();
            UIManager.Instance.PatchUIInstacne.UpdatePatch(downstatus.DownloadedBytes, downstatus.TotalBytes, downstatus.Percent);
            yield return null;
        }
    }
    #endregion
}
