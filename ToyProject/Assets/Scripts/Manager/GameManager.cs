using System.Collections;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
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
        ChangeGameState += ChangeState;
    }

    public GameState State { get; private set; }
    /// <summary>
    /// 게임 상태 변경 (Enum타입)
    /// </summary>
    /// <param name="GameState"></param>
    public Action<GameState> ChangeGameState;
    void ChangeState(GameState _state)
    {
        State = _state;
        switch(_state)
        {   
            case GameState.Login:
                break;
            case GameState.Lobby:
                break;
            case GameState.Playing:
                break;
            default:
                {
                    Debug.LogError("GaemeState Error (Enum Default)");
                }
                break;

        }
    }
    #region Patch    
    // 업데이스 코루틴 정지를 위한 변수
    Coroutine updateUI;
    // UIPatchButton에 등록할 함수

    public void CheckPatch(Action _openLogin, Action<long> _openPatch)
    {
        StartCoroutine(CoPatchCheck(_openLogin,_openPatch));

    }
    public void DownloadPatch(Action<long,long,float> _uiUpdate)
    {
        StartCoroutine(CoPatchDownLoad(_uiUpdate));
    }
    IEnumerator CoPatchCheck(Action _openLogin, Action<long> _openPatch)
    {
        yield return AuthManager.Instance.Init();

        AsyncOperationHandle<long> sizeCheck = Addressables.GetDownloadSizeAsync("Patch");        
        yield return sizeCheck;        
        if (sizeCheck.Result == 0)
        {
            _openLogin();
        }
        else
        {
            _openPatch(sizeCheck.Result);            
        }
    }
    IEnumerator CoPatchDownLoad(Action<long,long,float> _downloadUI)
    {   
        AsyncOperationHandle patch = Addressables.DownloadDependenciesAsync("Patch", true);
        updateUI = StartCoroutine(UpdateUI(patch, _downloadUI));
        yield return patch;
        StopCoroutine(updateUI);        
    }
    IEnumerator UpdateUI(AsyncOperationHandle _handle,Action<long, long, float> _downloadUI)
    {
        while(true)
        {
            yield return null;
            DownloadStatus downstatus = _handle.GetDownloadStatus();
            _downloadUI(downstatus.DownloadedBytes, downstatus.TotalBytes, downstatus.Percent);
        }
    }
    #endregion

}
