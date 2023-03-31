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
    public FirebaseUser User { get; private set; }    
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

    public async void SendFriendRequest(string _targetUID)
    {
        DataSnapshot userSnapshot = await Reference.Child("User").Child(_targetUID).GetValueAsync();

        if(userSnapshot.Exists)
        {
            var task = Reference.Child("User").Child(_targetUID).Child("Push").Child("FriendRequest").Child(User.UserId).SetValueAsync(true);
        }
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
    public void CreateRoom(string _roomInfo)
    {   
        UIManager.uiManager.OnDontClick();
        Reference.Child("Room").Child(_roomInfo).Child("Master").SetValueAsync(User.UserId).ContinueWithOnMainThread(
            (task)=>
            {
                UIManager.uiManager.OFFDontClick();
                if (task.IsCompleted)
                {   
                    UpdateRoom(_roomInfo);
                }
                else
                {
                    Debug.Log("AuthManger :: CreateRoomError");
                }
            });
    }
    public Action<FBRoomData,bool> AUpdateRoom { get; set; }
    public void UpdateRoom(string _roomName)
    {
        if (AUpdateRoom == null)
            return;

        Reference.Child("Room").Child(_roomName).GetValueAsync().ContinueWithOnMainThread(
            (task) =>
            { 
                if(task.IsCompleted)
                {
                    if(task.Result.Exists)
                    {
                        FBRoomData data = JsonUtility.FromJson<FBRoomData>(task.Result.GetRawJsonValue());
                        bool isMaster = (data.Master == User.UserId) ? true : false;
                        AUpdateRoom(data, isMaster);
                    }
                    else
                    {
                        UIManager.uiManager.OnErrorMessage("방이 존재하지 않습니다.");
                        UIManager.uiManager.CurrentPop();
                    }
                }
                else
                {

                }
            });
    }
    public void JoinRoom(string _roomName)
    {
        Reference.Child("Room").Child(_roomName).GetValueAsync().ContinueWithOnMainThread(
            (task)=>
            {

            });
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
        UpdateRoom(e.Snapshot.Key);
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
                UIManager.uiManager.OFFDontClick();
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
