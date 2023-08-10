using System.Collections;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine;
using Photon.Pun;
using System;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks, IPunOwnershipCallbacks
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

    private int photonViewNumber = 1000;

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
    int ReadyCount = 0;
    async void GameSetting()
    {
        UIManager.Instance.LoadingUIInstance.OpenLoadingUI(true);
        UIManager.Instance.LoadingUIInstance.ProgressSetting(3);

        await AuthManager.Instance.SettingTable();          // Table
        UIManager.Instance.LoadingUIInstance.CurrentStep++;

        await ResourceManager.Instance.PrefabSetting();     // Prefab 
        UIManager.Instance.LoadingUIInstance.CurrentStep++;
        Stack<PhotonView> pvStack =  SettingMap_Character(); 
        SettingMap_Castle();

        UIManager.Instance.ChangeUINavgation(GameState.Playing);
        UIManager.Instance.LoadingUIInstance.CurrentStep++;
        photonView.RPC("ReadyPlayer", RpcTarget.MasterClient);

        if (PhotonNetwork.IsMasterClient)
        {
            while(ReadyCount != PhotonNetwork.CurrentRoom.PlayerCount)
            {
                await System.Threading.Tasks.Task.Delay(1000);
            }
            ReadyCount = 0;
            SettingCharacterOwner(pvStack);
            photonView.RPC("CloseLoadingUI", RpcTarget.All);
            CountingNextRound();
        }
    }
    
    void SettingCharacterOwner(Stack<PhotonView> _characterStack)
    {
        Photon.Realtime.Player[] playerList = PhotonNetwork.PlayerList;
        GameObject[] startPos = GameObject.FindGameObjectsWithTag("StartPos");
        for (int i=0;i<playerList.Length;i++)
        {   
            PhotonView pv = _characterStack.Pop();
            object[] para = new object[] { pv.ViewID, startPos[i].transform.position };
            photonView.RPC("TransSetting", RpcTarget.All, para);
            pv.TransferOwnership(playerList[i]);
        }
    }
 
    void SettingMap_Castle()
    {
        GameObject castle = ResourceManager.Instance.GetPrefab("Castle");
        castle.transform.tag = "Castle";

        castle.transform.position = new Vector3(75, 0, 75);
        //castle script add

    }
    Stack<PhotonView> SettingMap_Character()
    {
        Stack<PhotonView> characterstack = new Stack<PhotonView>();
        Photon.Realtime.Player[] playerList= PhotonNetwork.PlayerList;
        for (int i = 0; i < playerList.Length; i++)
        {
            GameObject prefab = ResourceManager.Instance.GetPrefab("Knight");// 다시 할것
            if (prefab != null)
            {
                Character temp = prefab.gameObject.AddComponent<Character>();                
                PhotonView pv = prefab.AddComponent<PhotonView>();
                temp.SettingCharacter(pv);
                pv.OwnershipTransfer = OwnershipOption.Takeover;
                pv.ViewID = photonViewNumber++;
                characterstack.Push(pv);
            }
        }
        return characterstack;
        
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
    void TransSetting(int _phtonViewNumber , Vector3 _pos)
    {
        PhotonView pv = PhotonView.Find(_phtonViewNumber);
        pv.transform.position = _pos;
    }
    
    [PunRPC]
    void ReadyPlayer() { ReadyCount++; }
    [PunRPC]
    void CreateCharacter(int _photonViewNumber,Vector3 _pos)
    {

    }
    [PunRPC]
    void PRCountNextRound(int _count) 
    {
        if (UIManager.Instance.ATimer != null)
        {
            UIManager.Instance.ATimer(_count);
        }
    }
    [PunRPC]
    void CloseLoadingUI(){ UIManager.Instance.LoadingUIInstance.CloseLoadingUI(); }
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

    public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
    {
        throw new NotImplementedException();
    }

    public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
    {
        if(targetView.Owner == PhotonNetwork.LocalPlayer)
        {
            targetView.RequestOwnership();
            Character character = targetView.GetComponent<Character>();
            InputManager.Instance.SetUnit(character);
            CameraManager.Instance.SetActiveCamera(character.gameObject);
        }
    }

    public void OnOwnershipTransferFailed(PhotonView targetView, Player senderOfFailedRequest)
    {
        throw new NotImplementedException();
    }

    #endregion
}
