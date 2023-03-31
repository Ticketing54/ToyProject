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
                    Debug.LogError("����� �� ����");
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
    /// UIȰ��ȭ �ÿ���
    /// </summary>
    public Action AFriendListClear { get; set; }
    /// <summary>
    /// UIȰ��ȭ�ÿ��� (NickName,UID)
    /// </summary>
    public Action<UserInfo> ACheckFriendRequests { get; set; }
    public Action AMarkingFriendButton { get; set; }
    public Action<UserInfo> AUserSetting { get; set; }
    public Action<UserInfo> AsettingRoomSlot { get; set; }
    
    public void LobbyMainSetting()
    {
        Reference.Child("User").Child(User.UserId).GetValueAsync().ContinueWithOnMainThread(
            (task) =>
            {
                if (task.IsCompleted)
                {
                    CheckFriendRequests();
                    if(AUserSetting != null)
                    {
                        UserInfo player = JsonUtility.FromJson<UserInfo>(task.Result.GetRawJsonValue());
                        AUserSetting(player);
                    }

                }
                else
                {
                    Debug.Log("AuthManager:: LobbyMainsetting() Error");
                    UIManager.uiManager.OnErrorMessage("�α��� ������ �ҷ��� �� �����ϴ�.");
                    GameManager.Instance.ChangeState(GameState.Login);
                }
            });
    }
    public void CheckFriendRequests()
    {
        Reference.Child("User").Child(User.UserId).Child("Push").Child("FriendRequest").GetValueAsync().ContinueWithOnMainThread(
           (task) =>
           {
               if (task.IsFaulted)
               {
                   Debug.Log("����");
               }
               else if (task.IsCompleted)
               {
                   if (task.Result.Exists)
                   {
                       if (ACheckFriendRequests == null)
                           return;

                       DataSnapshot friends = task.Result;
                       foreach (DataSnapshot datasnapshot in friends.Children)
                       {
                         
                           FindUser_UID(datasnapshot.Key, ACheckFriendRequests);
                       }
                   }
               }
           });
    }
    /// <summary>
    /// FindUserList (SearchNickName)
    /// </summary>
    /// <param name="SearchNickName"></param>
    /// <param name="Action(NickName,UID)"></param>
    public void FindUserList_NickName(string _nickName, Action<UserInfo> _uiUpdate)
    {
        Reference.Child("User").OrderByChild("NickName").EqualTo(_nickName).GetValueAsync().ContinueWithOnMainThread(
           (task) =>
           {
               if (task.IsFaulted)
               {
                   Debug.Log("����");
               }
               else if (task.IsCompleted)
               {
                   DataSnapshot snapshot = task.Result; // ã�� ���� ���

                   Reference.Child("User").Child(User.UserId).Child("Friend").GetValueAsync().ContinueWithOnMainThread(
                       (friend) =>
                       {
                           DataSnapshot friends = friend.Result;
                           foreach (DataSnapshot child in snapshot.Children)
                           {
                               if (child.Key == User.UserId)
                                   continue;

                               bool isFriend = false;
                               foreach (DataSnapshot f in friends.Children)
                               {
                                   if(f.Key == child.Key )
                                   {
                                       isFriend = true;
                                       break;
                                   }
                               }
                               if(!isFriend)
                               {
                                   FindUser_UID(child.Key, _uiUpdate);
                               }
                           }
                       });
                   
               }
           });
    }
    /// <summary>
    /// UID�� ����ã��  �г����� ���°�츦 �����ϱ�
    /// </summary>
    /// <param name="UID"></param>
    /// <param name="Action(UID,NickName)"></param>
    public void FindUser_UID(string _uid, Action<UserInfo> _ui)
    {
        Reference.Child("User").Child(_uid).GetValueAsync().ContinueWithOnMainThread(
           (task) =>
           {
               if (task.IsFaulted)
               {
                   Debug.Log("����");
               }
               else if (task.IsCompleted)
               {
                   if (task.Result.Exists)
                   {
                       DataSnapshot userdata = task.Result;
                       UserInfo userinfo = JsonUtility.FromJson<UserInfo>(userdata.GetRawJsonValue());
                       _ui(userinfo);
                   }
               }
           });
    }
    public void SendFriendRequest(string _targetUID)
    {
        Reference.Child("User").Child(User.UserId).GetValueAsync().ContinueWithOnMainThread(
            (task) =>
            {
                if (task.IsCompleted)
                {
                    Reference.Child("User").Child(_targetUID).Child("Push").Child("FriendRequest").Child(User.UserId).SetValueAsync(true);
                }
                else
                {
                    UIManager.uiManager.OnErrorMessage("�˼����� ������ �߻��߽��ϴ�.");
                    Debug.Log(task.Exception);
                }
            });
    }
    public void RespondToFriendRequest(string _userID, string _nickName, bool _addFriend)
    {
        Reference.Child("User").Child(User.UserId).Child("Push").Child("FriendRequest").Child(_userID).RemoveValueAsync().ContinueWithOnMainThread(
           (task) =>
           {
               if (task.IsCompleted)
               {
                   if (_addFriend)
                   {
                       Reference.Child("User").Child(User.UserId).Child("Friend").Child(_userID).SetValueAsync(true);
                       Reference.Child("User").Child(_userID).Child("Friend").Child(User.UserId).SetValueAsync(true);
                   }
               }
               else
               {
                   UIManager.uiManager.OnErrorMessage("�˼����� ������ �߻��߽��ϴ�.");
                   Debug.Log(task.Exception);
               }
           });
    }
    public void UpdateFriendList()
    {
        Reference.Child("User").Child(User.UserId).Child("Friend").GetValueAsync().ContinueWithOnMainThread(
            (task) =>
            {
                if (task.IsCompleted)
                {
                    DataSnapshot friendsData = task.Result;
                    if (friendsData.Exists)
                    {
                        foreach (DataSnapshot friend in friendsData.Children)
                        {
                            FindUser_UID(friend.Key, AFriendAdd);
                        }
                    }
                }
                else
                {
                    Debug.Log("(AuthManager)UpdateFriendList Error");
                }
            });
    }
    public void CreateRoom(Action _openRoom)
    {
        UIManager.uiManager.OnDontClick();
        FBRoomData newRoom = new FBRoomData(User.UserId);
        Reference.Child("Room").Child(User.UserId).SetRawJsonValueAsync(JsonUtility.ToJson(newRoom)).ContinueWithOnMainThread(
            (task)=>
            {
                UIManager.uiManager.OFFDontClick();
                if (task.IsCompleted)
                {
                    _openRoom();
                    UpdateRoom(User.UserId);
                    Reference.Child("Room").Child(User.UserId).ValueChanged += RoomHandle;
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

        Reference.Child("Room").Child(_roomName).GetValueAsync().ContinueWith(
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
                        UIManager.uiManager.OnErrorMessage("���� �������� �ʽ��ϴ�.");
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
    public void AddEventListner()
    {
        Reference.Child("User").Child(User.UserId).Child("Push").Child("FriendRequest").ValueChanged += HandleFriendRequestChanged;
        Reference.Child("User").Child(User.UserId).Child("Friend").ValueChanged += HandleFriend;
    }

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
        FindUser_UID(e.Snapshot.Key, ACheckFriendRequests);
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
    /// Login : ID�� Password, ��������� UIǥ���Լ�
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
                    UIManager.uiManager.OnErrorMessage("�α��� ���");
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
                                    
                                    UIManager.uiManager.OnErrorMessage("�������� �ʴ� ID �Դϴ�");
                                    
                                }
                                break;
                            case AuthError.WrongPassword:
                                {
                                    UIManager.uiManager.OnErrorMessage("Password�� �ٽ��Է��� �ּ���");
                                    
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
                    Reference.Child("User").Child(User.UserId).GetValueAsync().ContinueWithOnMainThread(
                        (task) =>
                        { 
                            if(task.IsFaulted || task.IsCanceled)
                            {
                                GameManager.Instance.ChangeState(GameState.Login);
                                UIManager.uiManager.OnErrorMessage("�α��� ������ �������� �ʽ��ϴ�.");
                                return;
                            }
                            // �̺�Ʈ ������ 
                            AddEventListner();

                            if(task.Result.Child("NickName").Exists)
                            {
                                GameManager.Instance.ChangeState(GameState.Lobby);
                            }
                            else
                            {
                                if (AOpenNickNameUI != null)
                                {
                                    AOpenNickNameUI();
                                }
                                UIManager.uiManager.OFFDontClick();
                            }
                        });
                }
            });
    }

    /// <summary>
    /// Regist : ID�� Password, ��������� UIǥ���Լ�
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
                    UIManager.uiManager.OnErrorMessage("ȸ�� ������ ��� �Ǿ����ϴ�.");
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

                                    UIManager.uiManager.OnErrorMessage("�̹� ���ǰ��ִ� ID�Դϴ�.");

                                }
                                break;
                            case AuthError.WeakPassword:
                                {
                                    UIManager.uiManager.OnErrorMessage("�н����尡 �ʹ� �����մϴ�.");

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
                    UIManager.uiManager.OnErrorMessage("NicName ��� ����");
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
