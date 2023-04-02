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
    

    public FirebaseAuth Auth { get; private set; }
    public FirebaseApp App { get; private set; }
    public FirebaseUser User { get; private set; }    // 나중에 다시 설정할 것 ( 접속 끊겼을떄 대응)
    public DatabaseReference Reference { get; private set; }
    public Action AOpenNickNameUI { get; set; }

    #region Lobby
    /// <summary>
    /// Action FriendChanged (UID,NickName)
    /// </summary>
    public Action<UserInfo> AFriendAdd { get; set; }
    /// <summary>
    /// UI활성화 시에만
    /// </summary>
    public Action AFriendListClear { get; set; }
    /// <summary>
    /// UI활성화시에만 (NickName,UID)
    /// </summary>
    public Action<UserInfo> ACheckFriendRequests { get; set; }
    public Action AMarkingFriendButton { get; set; }
    public Action<UserInfo> AUserSetting { get; set; }
    public Action<UserInfo> AsettingRoomSlot { get; set; }
    
    public async void LobbyMainSetting()
    {
        DataSnapshot data = await Reference.Child("User").Child(User.UserId).GetValueAsync();
        if(data.Exists)
        {
            CheckFriendRequests();
            if (AUserSetting != null)
            {
                UserInfo player = JsonUtility.FromJson<UserInfo>(data.GetRawJsonValue());
                AUserSetting(player);
            }
        }
        else
        {
            Debug.Log("AuthManager:: LobbyMainsetting() Error");
            UIManager.uiManager.OnErrorMessage("로그인 정보를 불러올 수 없습니다.");
            GameManager.Instance.ChangeState(GameState.Login);
        }
    }
    public async void CheckFriendRequests()
    {
        DataSnapshot data = await Reference.Child("User").Child(User.UserId).Child("Push").Child("FriendRequest").GetValueAsync();
        if(data.Exists&& ACheckFriendRequests != null)
        {
            foreach(DataSnapshot request in data.Children)
            {
                DataSnapshot userSnapshot =  await Reference.Child("User").Child(request.Key).GetValueAsync();
                UserInfo userinfo = JsonUtility.FromJson<UserInfo>(userSnapshot.GetRawJsonValue());
                ACheckFriendRequests(userinfo);
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

    public void SendFriendRequest(string _targetUID)
    {
        Reference.Child("User").Child(_targetUID).Child("Push").Child("FriendRequest").Child(User.UserId).SetValueAsync(true);
    }
    public async void RespondToFriendRequest(string _userID,bool _addFriend)
    {
        await Reference.Child("User").Child(User.UserId).Child("Push").Child("FriendRequest").Child(_userID).RemoveValueAsync();
        if(_addFriend)
        {
            await Reference.Child("User").Child(User.UserId).Child("Friend").Child(_userID).SetValueAsync(true);
            await Reference.Child("User").Child(_userID).Child("Friend").Child(User.UserId).SetValueAsync(true);
        }
    }
    public async void UpdateFriendList()
    {
        DataSnapshot friendsSnapshot= await Reference.Child("User").Child(User.UserId).Child("Friend").GetValueAsync();
        if(friendsSnapshot.Exists)
        {
            foreach (DataSnapshot friend in friendsSnapshot.Children)
            {
                DataSnapshot friendSnapshot = await Reference.Child("User").Child(friend.Key).GetValueAsync();
                UserInfo userinfo = JsonUtility.FromJson<UserInfo>(friendSnapshot.GetRawJsonValue());
                AFriendAdd(userinfo);
            }
        }
    }
   
    #endregion

    #region FirebaseDataBase Handler
   
    private void HandleFriend(object sender, ValueChangedEventArgs e)
    {
        if (AFriendListClear == null || AFriendAdd == null)
            return;
        AFriendListClear();

        if (e.DatabaseError != null)
        {
            Debug.LogError(e.DatabaseError.Message);
            return;
        }
        UpdateFriendList();
    }
    private void HandleFriendRequestChanged(object sender, ValueChangedEventArgs e)
    {
        if (AMarkingFriendButton != null)
            AMarkingFriendButton();

        if (ACheckFriendRequests == null)
            return;
        if (e.DatabaseError != null)
        {
            Debug.LogError(e.DatabaseError.Message);
            return;
        }
        CheckFriendRequests();
    }
    private void RoomHandle(object sender, ValueChangedEventArgs e)
    {
        if (e.DatabaseError != null)
        {
            Debug.LogError(e.DatabaseError.Message);
            return;
        }
        UpdateRoom(e.Snapshot);
    }

    #endregion

    #region Auth User

    /// <summary>
    /// Login : ID와 Password, 결과에대한 UI표시함수
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
    async void LoginFirebase()
    {
        await Reference.Child("User").Child(User.UserId).Child("Connect").SetValueAsync(true);
        DataSnapshot snapshot = await Reference.Child("User").Child(User.UserId).GetValueAsync();
        if(snapshot.Exists)
        {
            UserInfo userinfo = JsonUtility.FromJson<UserInfo>(snapshot.GetRawJsonValue());
            Reference.Child("User").Child(User.UserId).Child("Push").Child("FriendRequest").ValueChanged += HandleFriendRequestChanged;
            Reference.Child("User").Child(User.UserId).Child("Friend").ValueChanged += HandleFriend;
            DataSnapshot friends = await Reference.Child("User").Child(User.UserId).Child("Friend").GetValueAsync();
            if(friends.Exists)
            {
                foreach (DataSnapshot friend in friends.Children)
                {
                    Reference.Child("User").Child(friend.Key).Child("Connect").ValueChanged += HandleFriend;
                }
            }
            if (userinfo.NickName == null && AOpenNickNameUI != null)
            {   
                AOpenNickNameUI();
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
    public void LogOut()
    {
        Reference.Child("User").Child(User.UserId).Child("Connect").SetValueAsync(false);
        User = null;
    }

    /// <summary>
    /// Regist : ID와 Password, 결과에대한 UI표시함수
    /// </summary>
    /// <param name="ID"></param>
    /// <param name="PW"></param>
    /// <param name="UIAction"></param>
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
    public void SetNickName(string _nicName)
    {
        UIManager.uiManager.OnDontClick();

        Reference.Child("User").Child(User.UserId).Child("NickName").SetValueAsync(_nicName).ContinueWithOnMainThread(
            (task) =>
            {
                if(task.IsCompleted)
                {
                    GameManager.Instance.ChangeState(GameState.Lobby);
                    UIManager.uiManager.OFFDontClick();
                }
                else
                {
                    UIManager.uiManager.OnErrorMessage("NicName 등록 오류");
                }
            });
    }

    #endregion
    #region Room
    public async void CreateRoom(string _roomInfo)
    {
        UIManager.uiManager.OnDontClick();
        await Reference.Child("Room").Child(_roomInfo).Child("Master").SetValueAsync(User.UserId);
        DataSnapshot _roomSnapshot = await Reference.Child("Room").Child(_roomInfo).GetValueAsync();
        UpdateRoom(_roomSnapshot);
        UIManager.uiManager.OFFDontClick();
    }
    public Action<List<UserInfo>> ARoomUpdate;

    public async void UpdateRoom(DataSnapshot _roomSnapshot)
    {
        if (ARoomUpdate == null)
            return;

        if (_roomSnapshot.Exists)
        {   
            //Reference.Child("Room").Child(_roomSnapShot.Key).ValueChanged -= ;
            List<UserInfo> userinfoList = new List<UserInfo>();
            DataSnapshot masterDs = await Reference.Child("User").Child(_roomSnapshot.Child("Master").Value.ToString()).GetValueAsync();
            UserInfo master = JsonUtility.FromJson<UserInfo>(masterDs.GetRawJsonValue());
            userinfoList.Add(master);
            DataSnapshot guestinfo = _roomSnapshot.Child("Guest");
            if (guestinfo.Exists)
            {
                foreach (DataSnapshot guestsanpShot in guestinfo.Children)
                {
                    DataSnapshot guestDs = await Reference.Child("User").Child(guestsanpShot.Key).GetValueAsync();
                    UserInfo guest = JsonUtility.FromJson<UserInfo>(guestDs.GetRawJsonValue());
                    userinfoList.Add(guest);
                }
            }
            ARoomUpdate(userinfoList);
        }
    }
    public void JoinRoom(string _roomName)
    {
        Reference.Child("Room").Child(_roomName).GetValueAsync().ContinueWithOnMainThread(
            (task) =>
            {

            });
    }
    public void SendInviteMessage(string _targetID)
    {
        Reference.Child("User").Child("Push").Child("RoomInviteRequest").Child(User.UserId).SetValueAsync(true);
    }
    
    /// <summary>
    /// RoomInviteList Update
    /// </summary>
    /// <param name="Action(UserInfo)"></param>
    async void UpdateInviteList(Action _add)
    {
        DataSnapshot friendsSnapshot = await Reference.Child("User").Child(User.UserId).Child("Friend").GetValueAsync();
        DataSnapshot roomSnapshot = await Reference.Child("Room").Child(User.UserId).GetValueAsync();
        if(roomSnapshot.Exists == false)
        {
            UIManager.uiManager.OnErrorMessage("방정보가 없습니다!!");
            return;
        }
        DataSnapshot guestsnapShot = roomSnapshot.Child("guest");
        Dictionary<string, object> guest = null;
        if (guestsnapShot.Exists)
        {
            guest = (Dictionary<string, object>)guestsnapShot.Value;
        }

        if(friendsSnapshot.Exists)
        {
            foreach(DataSnapshot friend in friendsSnapshot.Children)
            {
                if (guest != null && guest.ContainsKey(friend.Key))
                    continue;
                DataSnapshot frienddata = await Reference.Child("User").Child(friend.Key).GetValueAsync();
                UserInfo friendinfo = JsonUtility.FromJson<UserInfo>(frienddata.GetRawJsonValue());
            }
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
