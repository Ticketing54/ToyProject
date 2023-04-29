using System.Collections;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine;
using Photon.Pun;
using System;
public class GameManager : MonoBehaviourPun
{
    public static GameManager Instance;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            PhotonView pv = gameObject.AddComponent<PhotonView>();
            pv.ViewID = 999;
            PhotonNetwork.AutomaticallySyncScene = true;
            DontDestroyOnLoad(Instance.gameObject);
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
                    GameSetting();
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
    async void GameSetting()
    {
        UIManager.Instance.LoadingUIInstance.OpenLoadingUI(true);
        UIManager.Instance.LoadingUIInstance.ProgressSetting(3);

        await AuthManager.Instance.SettingTable();          // Table
        UIManager.Instance.LoadingUIInstance.CurrentStep++;

        await ResourceManager.Instance.PrefabSetting();     // Prefab 
        UIManager.Instance.LoadingUIInstance.CurrentStep++;
        SettingMap();
        UIManager.Instance.ChangeUINavgation(GameState.Playing);
        UIManager.Instance.LoadingUIInstance.CurrentStep++;
        if(PhotonNetwork.IsMasterClient)
        {
            CountingNextRound();
        }
    }
    void SettingMap()
    {
        SettingMap_Character();
        SettingMap_Castle();
        
        
    }
    void SettingMap_Castle()
    {
        GameObject castle = ResourceManager.Instance.GetPrefab("Castle");
        castle.transform.tag = "Castle";
        castle.transform.position = new Vector3(75, 0, 75);
        //castle script add

    }
    void SettingMap_Character()
    {
         Photon.Realtime.Player[] playerList= PhotonNetwork.PlayerList;

        GameObject[] spawner = GameObject.FindGameObjectsWithTag("Spawner");
        for (int i = 0; i < playerList.Length; i++)
        {
            GameObject character = ResourceManager.Instance.GetPrefab("Knight");// 다시 할것
            if (character != null)
            {
                character.transform.localScale = new Vector3(1f, 1f, 1f);
                character.transform.position = spawner[i].transform.position;
                //Character script add
                if (playerList[i].CustomProperties["UID"].ToString() == AuthManager.Instance.User.UserId)
                {
                    
                    Character temp = character.AddComponent<Character>();
                    temp.SettingCharacter();
                    InputManager.Instance.SetUnit(temp);
                    CameraManager.Instance.TargetPlayer(temp.cameraSet);
                }
            }

        }
    }
    /// <summary>
    /// Only MasterClient 
    /// </summary>
    void CountingNextRound() { StartCoroutine(CoCountNextRound()); }
    
    IEnumerator CoCountNextRound()
    {
        int count = 60;
        while (0 != count--)
        {
            photonView.RPC("PRCountNextRound", RpcTarget.All,count);
            yield return new WaitForSeconds(1f);
        }
        /// 다음 라운드
    }
    #region RPC

    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient == false)
            return;
    }
    public void ChangePlay() { photonView.RPC("PRChangePlayGame", RpcTarget.All); }
    public void OpenLobbyCounter() { photonView.RPC("PROpenLobbyCounter", RpcTarget.All); }
    public void LobbyCounterCounting(int _countNumber) { photonView.RPC("PRLobbyCounterCounting", RpcTarget.All, _countNumber); }
    [PunRPC]
    void PRCountNextRound(int _count) 
    {
        if (UIManager.Instance.ATimer != null)
        {
            UIManager.Instance.ATimer(_count);
        }
    }
    [PunRPC]
    void PRChangePlayGame() { ChangeState(GameState.Playing); }
    [PunRPC]
    void PROpenLobbyCounter() { UIManager.Instance.AOpenCounter(); }

    [PunRPC]
    void PRLobbyCounterCounting(int _countNumber)
    {
        UIManager.Instance.ACountNumber(_countNumber);
        if (_countNumber == -1)
        {
            UIManager.Instance.LoadingUIInstance.OpenLoadingUI();
        }
    }

    #endregion

    IEnumerator Counting()
    {
        float timer = 20f;
        while(timer>0)
        {   
            timer -= Time.deltaTime;
            if (UIManager.Instance.ATimer!= null)
                UIManager.Instance.ATimer(timer);
            yield return null;
        }
        if (UIManager.Instance.ATimer != null)
            UIManager.Instance.ATimer(0);
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
            UIManager.Instance.PatchUIInstacne.ClosePatchUI();
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
