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
    public FirebaseUser User { get; private set; }    // 나중에 다시 설정할 것 ( 접속 끊겼을떄 대응)
    public DatabaseReference Reference { get; private set; }


    /// <summary>
    /// FireBase접속 초기설정
    /// </summary> 
    /// <returns></returns>
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
                }
                else
                {
                    Debug.LogError("사용할 수 없음");
                    Application.Quit();
                }
            });
    }

    #region Lobby
    /// <summary>
    /// 로비 User정보를 얻어 초기세팅
    /// </summary>
    public async void LobbyMainSetting()
    {
        DataSnapshot data = await Reference.Child("User").Child(User.UserId).GetValueAsync();
        if(data.Exists)
        {
            CheckFriendRequests();
            if (UIManager.uiManager.ALobbyPlayerSetting != null)
            {
                UserInfo player = JsonUtility.FromJson<UserInfo>(data.GetRawJsonValue());
                UIManager.uiManager.ALobbyPlayerSetting(player);
            }
        }
        else
        {
            Debug.Log("AuthManager:: LobbyMainsetting() Error");
            UIManager.uiManager.OnErrorMessage("로그인 정보를 불러올 수 없습니다.");
            GameManager.Instance.ChangeState(GameState.Login);
        }
    }
    /// <summary>
    /// Push알람 체크 확인용
    /// </summary>
    public async void CheckFriendRequests()         // 나중에 PushCheck로 바꿀것
    {
        DataSnapshot data = await Reference.Child("User").Child(User.UserId).Child("Push").Child("FriendRequest").GetValueAsync();
        if(data.Exists&& UIManager.uiManager.ACheckFriendRequests != null)
        {
            foreach(DataSnapshot request in data.Children)
            {
                DataSnapshot userSnapshot =  await Reference.Child("User").Child(request.Key).GetValueAsync();
                UserInfo userinfo = JsonUtility.FromJson<UserInfo>(userSnapshot.GetRawJsonValue());
                UIManager.uiManager.ACheckFriendRequests(userinfo);
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
            if (friendSnapshot.Exists && friends.ContainsKey(user.Key))
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
        if (UIManager.uiManager.AFriendAdd == null)
            return;
        DataSnapshot friendsSnapshot= await Reference.Child("User").Child(User.UserId).Child("Friend").GetValueAsync();
        if(friendsSnapshot.Exists)
        {
            foreach (DataSnapshot friend in friendsSnapshot.Children)
            {
                DataSnapshot friendSnapshot = await Reference.Child("User").Child(friend.Key).GetValueAsync();
                UserInfo userinfo = JsonUtility.FromJson<UserInfo>(friendSnapshot.GetRawJsonValue());
                UIManager.uiManager.AFriendAdd(userinfo);
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
        UIManager.uiManager.AOpenInvitationMessage(user, roomName);
    }
    #endregion

    #region FirebaseDataBase Handler

    
    private void HandleInviteRequest(object sender, ChildChangedEventArgs e)
    {
        DataSnapshot invite = e.Snapshot;
        if (!invite.Exists)
            return;
        Reference.Child("User").Child(User.UserId).Child("Push").Child("RoomInviteRequest").Child(invite.Key).RemoveValueAsync();
        if (UIManager.uiManager.AOpenInvitationMessage != null)
        {
            ReceiveinvitationMessage(invite);
        }
    }
    private void HandleFriend(object sender, ValueChangedEventArgs e)
    {
        if (UIManager.uiManager.AFriendListClear == null || UIManager.uiManager.AFriendAdd == null)
            return;
        UIManager.uiManager.AFriendListClear();

        if (e.DatabaseError != null)
        {
            Debug.LogError(e.DatabaseError.Message);
            return;
        }
        UpdateFriendList();
    }
    private void HandleFriendRequestChanged(object sender, ValueChangedEventArgs e)
    {

        if (UIManager.uiManager.AMarkingFriendButton != null)
            UIManager.uiManager.AMarkingFriendButton();

        if (UIManager.uiManager.ACheckFriendRequests == null)
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
    /// Login : ID와 Password
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
        UIManager.uiManager.OnDontClick();
        Auth.SignInWithEmailAndPasswordAsync(_id, _pw).ContinueWithOnMainThread(
            (task) =>
            {
                if(task.IsCanceled)
                {   
                    Debug.Log("Login Canceled");
                    UIManager.uiManager.OnErrorMessage("로그인 취소");
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
                                    
                                    UIManager.uiManager.OnErrorMessage("존재하지 않는 ID 입니다");
                                    
                                }
                                break;
                            case AuthError.WrongPassword:
                                {
                                    UIManager.uiManager.OnErrorMessage("Password를 다시입력해 주세요");
                                    
                                }
                                break;
                            default:
                                UIManager.uiManager.OnErrorMessage(error.ToString());
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
    /// 사용자의 FirebaseDatabase 정보확인
    /// </summary>
    async void LoginFirebase()
    {
        await Reference.Child("User").Child(User.UserId).Child("Connect").SetValueAsync(true);
        DataSnapshot snapshot = await Reference.Child("User").Child(User.UserId).GetValueAsync();
        if(snapshot.Exists)
        {
            UserInfo userinfo = JsonUtility.FromJson<UserInfo>(snapshot.GetRawJsonValue());
            // 핸들러 등록
            Reference.Child("User").Child(User.UserId).Child("Push").Child("FriendRequest").ValueChanged        += HandleFriendRequestChanged;
            Reference.Child("User").Child(User.UserId).Child("Friend").ValueChanged                             += HandleFriend;

            await Reference.Child("User").Child(User.UserId).Child("Push").Child("RoomInviteRequest").SetValueAsync(true); // 초대 메세지 초기화
            Reference.Child("User").Child(User.UserId).Child("Push").Child("RoomInviteRequest").ChildAdded += HandleInviteRequest;
            DataSnapshot friends = await Reference.Child("User").Child(User.UserId).Child("Friend").GetValueAsync();
            if(friends.Exists)
            {
                foreach (DataSnapshot friend in friends.Children)
                {
                    Reference.Child("User").Child(friend.Key).Child("Connect").ValueChanged += HandleFriend;
                }
            }
            
            if(UIManager.uiManager.AOpenNickNameUI==null)
            {
                GameManager.Instance.ChangeState(GameState.Login);
            }
            else if (userinfo.NickName == null)
            {

                UIManager.uiManager.AOpenNickNameUI();
            }
            else
            {
                GameManager.Instance.ChangeState(GameState.Lobby);
            }
        }
        else
        {
            GameManager.Instance.ChangeState(GameState.Login);
            UIManager.uiManager.OnErrorMessage("로그인 정보가 존재하지 않습니다.");
        }
        UIManager.uiManager.OFFDontClick();
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
    /// Regist : ID와 Password
    /// </summary>
    /// <param name="ID"></param>
    /// <param name="PW"></param>
    public void Regist(string _id,string _pw)
    {
        UIManager.uiManager.OnDontClick();

        Auth.CreateUserWithEmailAndPasswordAsync(_id, _pw).ContinueWithOnMainThread(
            (task) =>
            {
                if (task.IsCanceled)
                {
                    Debug.Log("Regist Canceled");
                    UIManager.uiManager.OnErrorMessage("회원 가입이 취소 되었습니다.");
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

                                    UIManager.uiManager.OnErrorMessage("이미 사용되고있는 ID입니다.");

                                }
                                break;
                            case AuthError.WeakPassword:
                                {
                                    UIManager.uiManager.OnErrorMessage("패스워드가 너무 간단합니다.");

                                }
                                break;
                            default:
                                {
                                    Debug.Log(error);
                                    UIManager.uiManager.OnErrorMessage(error.ToString());
                                }                                
                                break;
                        }
                    }
                }
                else
                {   
                    Reference.Child("User").Child(task.Result.UserId).Child("UID").SetValueAsync(task.Result.UserId);
                    Reference.Child("User").Child(task.Result.UserId).Child("Connect").SetValueAsync(false);
                    UIManager.uiManager.OFFDontClick();
                    UIManager.uiManager.CurrentPop();
                }
            });
    }
    /// <summary>
    /// NickNameSetting
    /// </summary>
    /// <param name="NickName"></param>
    public async void SetNickName(string _nicName)
    {
        UIManager.uiManager.OnDontClick();
        await Reference.Child("User").Child(User.UserId).Child("NickName").SetValueAsync(_nicName);
        UIManager.uiManager.OFFDontClick();
        GameManager.Instance.ChangeState(GameState.Lobby);
    }

    #endregion
    #region Room
    /// <summary>
    /// CreateRoom with FirebaseDataBase
    /// </summary>
    /// <param name="RoomInfo"></param>
    public async void CreateRoom(string _roomInfo)
    {
        UIManager.uiManager.OpenLoadingUI();
        if (UIManager.uiManager.AOpenRoom == null)
        {
            UIManager.uiManager.CloseLoadingUI();
            return;
        }
        UIManager.uiManager.AOpenRoom();

        await Reference.Child("Room").Child(_roomInfo).Child("Master").SetValueAsync(User.UserId);
        DataSnapshot roomSnapshot = await Reference.Child("Room").Child(_roomInfo).GetValueAsync();
        
        if (roomSnapshot.Exists && UIManager.uiManager.ARoomUpdate != null)
        {
            Queue<UserInfo> userinfoQ = new Queue<UserInfo>();
            DataSnapshot masterDs = await Reference.Child("User").Child(roomSnapshot.Child("Master").Value.ToString()).GetValueAsync();
            UserInfo master = JsonUtility.FromJson<UserInfo>(masterDs.GetRawJsonValue());
            userinfoQ.Enqueue(master);
            UIManager.uiManager.ARoomUpdate(userinfoQ);
        }

        UIManager.uiManager.CloseLoadingUI();
    }
    public async void JoinRoom(string _roomName, List<string> _userUID, bool _isEntered)
    {
        await Reference.Child("Room").Child(_roomName).Child("Guest").Child(User.UserId).SetValueAsync(true);
        UpdateRoom(_roomName, _userUID, _isEntered);
    }
    /// <summary>
    /// RoomStateUpdate
    /// </summary>
    /// <param name="RoomDataSnapshot"></param>
    public async void UpdateRoom(string _roomName,List<string> _userUID,bool _isEntered)
    {
        Queue<UserInfo> userQ = new Queue<UserInfo>();
        DataSnapshot roomInfo = await Reference.Child("Room").Child(_roomName).GetValueAsync();
        DataSnapshot masterDs = await Reference.Child("User").Child(roomInfo.Child("Master").Value.ToString()).GetValueAsync();
        UserInfo masterinfo = JsonUtility.FromJson<UserInfo>(masterDs.GetRawJsonValue());
        userQ.Enqueue(masterinfo);

        foreach(DataSnapshot guest in roomInfo.Child("Guest").Children)
        {
            if (masterinfo.UID == guest.Key)
                continue;
            DataSnapshot guestDs = await Reference.Child("User").Child(guest.Key).GetValueAsync();
            UserInfo guestinfo = JsonUtility.FromJson<UserInfo>(guestDs.GetRawJsonValue());
            userQ.Enqueue(guestinfo);
        }
        UIManager.uiManager.ARoomUpdate(userQ);

        if(!_isEntered)
            UIManager.uiManager.CloseLoadingUI();
    }
    /// <summary>
    ///  DestroyRoom
    /// </summary>
    /// <param name="RoomName"></param>
    public void DestroyRoom(string _roomName) { Reference.Child("Room").Child(_roomName).RemoveValueAsync(); }
    /// <summary>
    /// JoinRoom with FirebaseDatabase
    /// </summary>
    /// <param name="RoomName"></param>

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
                    Debug.Log("취소");
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
