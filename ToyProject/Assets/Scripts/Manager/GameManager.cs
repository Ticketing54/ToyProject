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

    private void Start()
    {
        State = GameState.Login;
        StartCoroutine(CoPatchCheck());        
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
                {
                    StartCoroutine(CoPatchCheck());
                }
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
    IEnumerator CoPatchCheck()
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
