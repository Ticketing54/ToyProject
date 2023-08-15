using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Storage;
using Firebase.Extensions;
using Firebase.Database;
using Photon.Pun;
using System;
using System.Threading.Tasks;
public class AuthManager 
{
    private static AuthManager instance;
    public static AuthManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new AuthManager();                
            }           

            return instance;
        }
    }

    public FirebaseAuth Auth { get; private set; }
    public FirebaseApp App { get; private set; }
    public FirebaseUser User { get; private set; }    // ���߿� �ٽ� ������ �� ( ���� �������� ����)
    public DatabaseReference Reference { get; private set; }

    #region Table
    /// <summary>
    /// ResourceManager TableSetting
    /// </summary>
    public async Task SettingTable()
    {
        DataSnapshot tableSnapShot = await Reference.Child("Table").GetValueAsync();
        Dictionary<int, StageInfo> stageDic = GetStageTable(tableSnapShot.Child("Stage"));
        Dictionary<string, MonsterInfo> monsterDic = GetMonsterTable(tableSnapShot.Child("Monster"));
        ResourceManager.Instance.SettingTable(stageDic, monsterDic);
    }
    /// <summary>
    /// StageTable
    /// </summary>
    /// <param name="SnapShot(Stage)"></param>
    /// <returns></returns>
    Dictionary<int, StageInfo> GetStageTable(DataSnapshot _dataSnapShot)
    {
        Dictionary<int, StageInfo> stageTable = new Dictionary<int, StageInfo>();
        DataSnapshot monstersSnapshot = _dataSnapShot.Child("Monster");

        foreach(DataSnapshot stage in _dataSnapShot.Children)
        {
            List<List<string>> mobList = new List<List<string>>();
            foreach(DataSnapshot monster in stage.Child("Monster").Children)
            {
                List<string> mobinfo = new List<string>();
                mobinfo.Add(monster.Key);
                mobinfo.Add(monster.Value.ToString());
                mobList.Add(mobinfo);
            }
            int stageNumber = int.Parse(stage.Key);
            int gold = int.Parse(stage.Child("Reward").Child("Gold").Value.ToString());
            int exp = int.Parse(stage.Child("Reward").Child("Exp").Value.ToString());
            StageInfo stageinfo = new StageInfo(gold, exp, mobList);
            stageTable.Add(stageNumber, stageinfo);
        } 
        return stageTable;
    }
    /// <summary>
    /// MonsterTable
    /// </summary>
    /// <param name="SnapShot(Monster)"></param>
    /// <returns></returns>
    Dictionary<string, MonsterInfo> GetMonsterTable(DataSnapshot _dataSnapShot)
    {
        Dictionary<string, MonsterInfo> monsterTable = new Dictionary<string, MonsterInfo>();
        foreach (DataSnapshot info in _dataSnapShot.Children)
        {
            MonsterInfo mobinfo = JsonUtility.FromJson<MonsterInfo>(info.GetRawJsonValue());
            monsterTable.Add(info.Key, mobinfo);
        }
        return monsterTable;
    }
    #endregion
    /// <summary>
    /// FireBase���� �ʱ⼳��
    /// </summary> 
    /// <returns></returns>
    /// 

    public IEnumerator Init()
    {
        User = null;
        yield return FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(
            (task) =>
            {
                if(task.Result == DependencyStatus.Available)
                {
                    Auth = FirebaseAuth.DefaultInstance;
                    App = FirebaseApp.DefaultInstance;
                    Reference = FirebaseDatabase.DefaultInstance.RootReference;
                    Application.quitting += OnApplicationQuit;
                }
                else
                {
                    Debug.LogError("����� �� ����");
                    Application.Quit();
                }
            });
    }

    async void OnApplicationQuit()
    {
        if (User == null)
            return;
        Task recordConnect = Reference.Child("User").Child(User.UserId).Child("Connect").SetValueAsync(false);
        Task wait = Task.Delay(5000);
        await wait;
       

        User = null;
    }
    #region Lobby
    /// <summary>
    /// �κ� User������ ��� �ʱ⼼��
    /// </summary>
    public async void LobbyMainSetting()
    {
        DataSnapshot data = await Reference.Child("User").Child(User.UserId).GetValueAsync();
        if(data.Exists)
        {
            CheckFriendRequests();
            if (UIManager.Instance.ALobbyPlayerSetting != null)
            {
                UserInfo player = JsonUtility.FromJson<UserInfo>(data.GetRawJsonValue());
                UIManager.Instance.ALobbyPlayerSetting(player);
            }
        }
        else
        {
            Debug.Log("AuthManager:: LobbyMainsetting() Error");
            UIManager.Instance.OnErrorMessage("�α��� ������ �ҷ��� �� �����ϴ�.");
            GameManager.Instance.ChangeState(GameState.Login);
        }
    }
    /// <summary>
    /// Push�˶� üũ Ȯ�ο�
    /// </summary>
    public async void CheckFriendRequests()         // ���߿� PushCheck�� �ٲܰ�
    {
        DataSnapshot data = await Reference.Child("User").Child(User.UserId).Child("Push").Child("FriendRequest").GetValueAsync();
        if(data.Exists&& UIManager.Instance.ACheckFriendRequests != null)
        {
            foreach(DataSnapshot request in data.Children)
            {
                DataSnapshot userSnapshot =  await Reference.Child("User").Child(request.Key).GetValueAsync();
                UserInfo userinfo = JsonUtility.FromJson<UserInfo>(userSnapshot.GetRawJsonValue());
                UIManager.Instance.ACheckFriendRequests(userinfo);
            }
        }
    }
    /// <summary>
    /// FindUserList (SearchNickName)
    /// </summary>
    /// <param name="SearchNickName"></param>
    /// <param name="Action(NickName,UID)"></param>
    public async void FindUserList_NickName(string _nickName, Action<UserInfo> _uiUpdate)
    {
        DataSnapshot findUser = await Reference.Child("User").OrderByChild("NickName").EqualTo(_nickName).GetValueAsync();
        DataSnapshot friendSnapshot = await Reference.Child("User").Child(User.UserId).Child("Friend").GetValueAsync();
        Dictionary<string, object> friends = (Dictionary<string, object>)friendSnapshot.Value;
        
        foreach (DataSnapshot user in findUser.Children)
        {
            if (user.Key == User.UserId)
                continue;
            if (friends != null && friends.ContainsKey(user.Key))
                continue;
            DataSnapshot usersnap = await Reference.Child("User").Child(user.Key).GetValueAsync();
            UserInfo userinfo = JsonUtility.FromJson<UserInfo>(usersnap.GetRawJsonValue());
            _uiUpdate(userinfo);
        }
    }
    /// <summary>
    /// SendRequest
    /// </summary>
    /// <param name="TargetUID"></param>
    public void SendFriendRequest(string _targetUID)
    {
        Reference.Child("User").Child(_targetUID).Child("Push").Child("FriendRequest").Child(User.UserId).SetValueAsync(true);
    }
    /// <summary>
    /// Respond to a friend request
    /// </summary>
    /// <param name="Sender"></param>
    /// <param name="Choice"></param>
    public async void RespondToFriendRequest(string _userID,bool _addFriend)
    {
        await Reference.Child("User").Child(User.UserId).Child("Push").Child("FriendRequest").Child(_userID).RemoveValueAsync();
        if(_addFriend)
        {
            await Reference.Child("User").Child(User.UserId).Child("Friend").Child(_userID).SetValueAsync(true);
            await Reference.Child("User").Child(_userID).Child("Friend").Child(User.UserId).SetValueAsync(true);
        }
    }
    /// <summary>
    /// UpdateFriendList with FirebaseDatabase
    /// </summary>
    public async void UpdateFriendList()
    {
        if (UIManager.Instance.AFriendAdd == null)
            return;
        DataSnapshot friendsSnapshot= await Reference.Child("User").Child(User.UserId).Child("Friend").GetValueAsync();
        if(friendsSnapshot.Exists)
        {
            foreach (DataSnapshot friend in friendsSnapshot.Children)
            {
                DataSnapshot friendSnapshot = await Reference.Child("User").Child(friend.Key).GetValueAsync();
                UserInfo userinfo = JsonUtility.FromJson<UserInfo>(friendSnapshot.GetRawJsonValue());
                UIManager.Instance.AFriendAdd(userinfo);
            }
        }
    }
    /// <summary>
    /// SendInvitationMessage
    /// </summary>
    /// <param name="TargetUserUID"></param>
    /// <param name="RoomName"></param>
    public void SendInvitationMessage(string _targetUID, string _roomName)
    {
        Reference.Child("User").Child(_targetUID).Child("Push").Child("RoomInviteRequest").Child(_roomName).SetValueAsync(User.UserId);
    }
    /// <summary>
    /// ReceiveinvitationMessage
    /// </summary>
    /// <param name="MessageDataSnapshot"></param>
    async void ReceiveinvitationMessage(DataSnapshot _inviteMessage)
    {
        DataSnapshot senderinfo = await Reference.Child("User").Child(_inviteMessage.Value.ToString()).GetValueAsync();
        UserInfo user = JsonUtility.FromJson<UserInfo>(senderinfo.GetRawJsonValue());
        string roomName = _inviteMessage.Key;
        UIManager.Instance.AOpenInvitationMessage(user, roomName);
    }
    #endregion

    #region FirebaseDataBase Handler

    
    private void HandleInviteRequest(object sender, ChildChangedEventArgs e)
    {
        DataSnapshot invite = e.Snapshot;
        if (!invite.Exists)
            return;
        Reference.Child("User").Child(User.UserId).Child("Push").Child("RoomInviteRequest").Child(invite.Key).RemoveValueAsync();
        if (UIManager.Instance.AOpenInvitationMessage != null)
        {
            ReceiveinvitationMessage(invite);
        }
    }
    private void HandleFriend(object sender, ValueChangedEventArgs e)
    {
        if (UIManager.Instance.AFriendListClear == null || UIManager.Instance.AFriendAdd == null)
            return;
        UIManager.Instance.AFriendListClear();

        if (e.DatabaseError != null)
        {
            Debug.LogError(e.DatabaseError.Message);
            return;
        }
        UpdateFriendList();
    }
    private void HandleFriendRequestChanged(object sender, ValueChangedEventArgs e)
    {

        if (UIManager.Instance.AMarkingFriendButton != null)
            UIManager.Instance.AMarkingFriendButton();

        if (UIManager.Instance.ACheckFriendRequests == null)
            return;
        if (e.DatabaseError != null)
        {
            Debug.LogError(e.DatabaseError.Message);
            return;
        }
        CheckFriendRequests();
    }
  
    #endregion

    #region Auth User

    /// <summary>
    /// Login : ID�� Password
    /// </summary>
    /// <param name="ID"></param>
    /// <param name="PW"></param>
    /// <param name="UIAction"></param>
    public void Login(string _id, string _pw)
    {
        if (Auth == null)
        {
            Debug.LogError("Auth is Null");
        }
        UIManager.Instance.OnDontClick();
        Auth.SignInWithEmailAndPasswordAsync(_id, _pw).ContinueWithOnMainThread(
            (task) =>
            {
                if(task.IsCanceled)
                {   
                    Debug.Log("Login Canceled");
                    UIManager.Instance.OnErrorMessage("�α��� ���");
                }
                else if (task.IsFaulted)
                {   
                    if(task.Exception.InnerException.InnerException is FirebaseException)
                    {
                        FirebaseException ex = (FirebaseException)task.Exception.InnerException.InnerException;
                        AuthError error = (AuthError)ex.ErrorCode;
                        switch (error)
                        {
                            case AuthError.UserNotFound:
                                {
                                    
                                    UIManager.Instance.OnErrorMessage("�������� �ʴ� ID �Դϴ�");
                                    
                                }
                                break;
                            case AuthError.WrongPassword:
                                {
                                    UIManager.Instance.OnErrorMessage("Password�� �ٽ��Է��� �ּ���");
                                    
                                }
                                break;
                            default:
                                UIManager.Instance.OnErrorMessage(error.ToString());
                                break;
                        }
                    }
                }
                else
                {   
                    User = task.Result;
                    LoginFirebase();
                }
            });
    }
    /// <summary>
    /// ������� FirebaseDatabase ����Ȯ��
    /// </summary>
    async void LoginFirebase()
    {
        await Reference.Child("User").Child(User.UserId).Child("Connect").SetValueAsync(true);
        DataSnapshot snapshot = await Reference.Child("User").Child(User.UserId).GetValueAsync();
        if(snapshot.Exists)
        {
            UserInfo userinfo = JsonUtility.FromJson<UserInfo>(snapshot.GetRawJsonValue());
            // �ڵ鷯 ���
            Reference.Child("User").Child(User.UserId).Child("Push").Child("FriendRequest").ValueChanged        += HandleFriendRequestChanged;
            Reference.Child("User").Child(User.UserId).Child("Friend").ValueChanged                             += HandleFriend;

            await Reference.Child("User").Child(User.UserId).Child("Push").Child("RoomInviteRequest").SetValueAsync(true); // �ʴ� �޼��� �ʱ�ȭ
            Reference.Child("User").Child(User.UserId).Child("Push").Child("RoomInviteRequest").ChildAdded += HandleInviteRequest;
            DataSnapshot friends = await Reference.Child("User").Child(User.UserId).Child("Friend").GetValueAsync();
            if(friends.Exists)
            {
                foreach (DataSnapshot friend in friends.Children)
                {
                    Reference.Child("User").Child(friend.Key).Child("Connect").ValueChanged += HandleFriend;
                }
            }
            
            if(UIManager.Instance.AOpenNickNameUI==null)
            {
                GameManager.Instance.ChangeState(GameState.Login);
            }
            else if (userinfo.NickName == null)
            {

                UIManager.Instance.AOpenNickNameUI();
            }
            else
            {
                GameManager.Instance.ChangeState(GameState.Lobby);
            }
        }
        else
        {
            GameManager.Instance.ChangeState(GameState.Login);
            UIManager.Instance.OnErrorMessage("�α��� ������ �������� �ʽ��ϴ�.");
        }
        UIManager.Instance.OFFDontClick();
    }

    /// <summary>
    /// Firebase Logout
    /// </summary>
    public void LogOut()
    {
        if (User == null)
            return;
        Reference.Child("User").Child(User.UserId).Child("Connect").SetValueAsync(false);
        User = null;
    }    
    /// <summary>
    /// Regist : ID�� Password
    /// </summary>
    /// <param name="ID"></param>
    /// <param name="PW"></param>
    public void Regist(string _id,string _pw)
    {
        UIManager.Instance.OnDontClick();

        Auth.CreateUserWithEmailAndPasswordAsync(_id, _pw).ContinueWithOnMainThread(
            (task) =>
            {
                if (task.IsCanceled)
                {
                    Debug.Log("Regist Canceled");
                    UIManager.Instance.OnErrorMessage("ȸ�� ������ ��� �Ǿ����ϴ�.");
                }
                else if (task.IsFaulted)
                {
                    if (task.Exception.InnerException.InnerException is FirebaseException)
                    {
                        FirebaseException ex = (FirebaseException)task.Exception.InnerException.InnerException;
                        AuthError error = (AuthError)ex.ErrorCode;
                        switch (error)
                        {
                            case AuthError.EmailAlreadyInUse:
                                {

                                    UIManager.Instance.OnErrorMessage("�̹� ���ǰ��ִ� ID�Դϴ�.");

                                }
                                break;
                            case AuthError.WeakPassword:
                                {
                                    UIManager.Instance.OnErrorMessage("�н����尡 �ʹ� �����մϴ�.");

                                }
                                break;
                            default:
                                {
                                    Debug.Log(error);
                                    UIManager.Instance.OnErrorMessage(error.ToString());
                                }                                
                                break;
                        }
                    }
                }
                else
                {   
                    Reference.Child("User").Child(task.Result.UserId).Child("UID").SetValueAsync(task.Result.UserId);
                    Reference.Child("User").Child(task.Result.UserId).Child("Connect").SetValueAsync(false);
                    UIManager.Instance.OFFDontClick();
                    UIManager.Instance.CurrentPop();
                }
            });
    }
    /// <summary>
    /// NickNameSetting
    /// </summary>
    /// <param name="NickName"></param>
    public async void SetNickName(string _nicName)
    {
        UIManager.Instance.OnDontClick();
        await Reference.Child("User").Child(User.UserId).Child("NickName").SetValueAsync(_nicName);
        UIManager.Instance.OFFDontClick();
        GameManager.Instance.ChangeState(GameState.Lobby);
    }

    #endregion

    #region Room
    /// <summary>
    /// RoomStateUpdate
    /// </summary>
    /// <param name="RoomDataSnapshot"></param>
    public async void UpdateRoom(string _roomName,List<string> _userUID)
    {
        Queue<UserInfo> userQ = new Queue<UserInfo>();
        DataSnapshot masterDs = await Reference.Child("User").Child(_userUID[0]).GetValueAsync();
        UserInfo masterinfo = JsonUtility.FromJson<UserInfo>(masterDs.GetRawJsonValue());
        userQ.Enqueue(masterinfo);

        foreach(string roomUserUID in _userUID)
        {
            if (masterinfo.UID == roomUserUID || roomUserUID == null)
                continue;
            DataSnapshot guestDs = await Reference.Child("User").Child(roomUserUID).GetValueAsync();
            UserInfo guestinfo = JsonUtility.FromJson<UserInfo>(guestDs.GetRawJsonValue());
            userQ.Enqueue(guestinfo);
        }
        if(UIManager.Instance.ARoomUpdate != null)
        {
            UIManager.Instance.ARoomUpdate(userQ);
        }
        
    }
    
    #endregion

    #region  Chat
    /// <summary>
    /// SendMessage (UserID, Message)
    /// </summary>
    /// <param name="Receiver"></param>
    /// <param name="_message"></param>
    public void OpenChat(string _recevier)
    {
        Reference.Child("Chat").Child(RoomName(_recevier)).GetValueAsync().ContinueWithOnMainThread(
            (task) =>
            {
                if(task.IsFaulted)
                {
                    Debug.Log("���");
                }
                else if (task.IsCompleted)
                {

                }
            });
        
    }
    string RoomName(string _recevier)
    {
        string roomName = string.Empty;
        for(int i=0;i<_recevier.Length;i++)
        {
            if(_recevier[i] > User.UserId[i])
            {
                roomName = User.UserId + "@" + _recevier;
                break;
            }
            else if (_recevier[i] > User.UserId[i])
            {
                roomName = _recevier + "@" + User.UserId;
                break;
            }
        }
        return roomName;
    }
  
    #endregion
}