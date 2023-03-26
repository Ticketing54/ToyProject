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
    public Action OpenNickNameUI { get; set; }

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
                                if (OpenNickNameUI != null)
                                {
                                    OpenNickNameUI();
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
                    Reference.Child("User").Child(task.Result.UserId).Child("Push").Child("FriendRequest").Child("Empty").SetValueAsync(true);
                    Reference.Child("User").Child(task.Result.UserId).Child("Friend").Child("Empty").SetValueAsync(true);
                    Reference.Child("User").Child(task.Result.UserId).Child("UserID").SetValueAsync(task.Result.UserId);
                    
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

    public void AddEventListner()
    {
        Reference.Child("User").Child(User.UserId).Child("Push").Child("FriendRequest").ChildAdded += HandleFriendRequestChanged;
        Reference.Child("User").Child(User.UserId).Child("Friend").Child("Empty").ChildAdded += HandleFriend;
    }
    /// <summary>
    /// Action FriendChanged (UID,NickName)
    /// </summary>
    public Action<string,string> AFriendUI { get; set; }
    private void HandleFriend(object sender, ChildChangedEventArgs e)
    {
        if (AFriendUI == null)
            return;
        if (e.DatabaseError != null)
        {
            Debug.LogError(e.DatabaseError.Message);
            return;
        }
        AFriendUI(e.Snapshot.Child("UID").Value.ToString(), e.Snapshot.Child("NickName").Value.ToString());
        throw new NotImplementedException();
    }


    /// <summary>
    /// Action FriendRequestChanged (UID,NickName)
    /// </summary>
    public Action<string, string> AFriendRequestUI { get; set; }
    void HandleFriendRequestChanged(object sender, ChildChangedEventArgs e)
    {
        if (AFriendRequestUI == null)
            return;

        if(e.DatabaseError != null)
        {
            Debug.LogError(e.DatabaseError.Message);
            return;
        }
        AFriendRequestUI(e.Snapshot.Key, e.Snapshot.Key);
    }
    /// <summary>
    /// UIȰ��ȭ �ÿ���
    /// </summary>
    public Action<UserInfo> AUpdateFriendsList { get; set; }
    /// <summary>
    /// UIȰ��ȭ�ÿ��� (NickName,UID)
    /// </summary>
    public Action<string,string> AUpdateFindUserUI { get; set; }
    
    public void CheckFriendRequests(Action<string,string> _ui)
    {
        Reference.Child("User").Child(User.UserId).Child("Push").Child("FriendRequest").OrderByChild("Send").GetValueAsync().ContinueWithOnMainThread(
           (task) =>
           {
               if (task.IsFaulted)
               {
                   Debug.Log("����");
               }
               else if (task.IsCompleted)
               {
                   DataSnapshot friends = task.Result;
                   foreach(DataSnapshot datasnapshot in friends.Children)
                   {
                       if(datasnapshot.Child("UID").Exists)
                       {
                           _ui(datasnapshot.Child("UID").Value.ToString(), datasnapshot.Child("NickName").Value.ToString());
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
    public void FindUserList_NickName(string _nickName, Action<string,string> _uiUpdate)
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
                   DataSnapshot snapshot = task.Result;
                   foreach (DataSnapshot child in snapshot.Children)
                   {
                       _uiUpdate(_nickName, child.Key);
                   }
               }
           });
    }
    public void SendFriendRequest(string _targetUID)
    {
        Reference.Child("User").Child(User.UserId).Child("NickName").GetValueAsync().ContinueWithOnMainThread(
            (task) =>
            {
                if (task.IsCompleted)
                {
                    Dictionary<string, object> request = new Dictionary<string, object>();
                    request["UID"] = User.UserId;
                    request["NickName"] = task.Result.Value.ToString();
                    Reference.Child("User").Child(_targetUID).Child("Push").Child("FriendRequest").Child(User.UserId).SetValueAsync(request);
                }
                else
                {
                    UIManager.uiManager.OnErrorMessage("�˼����� ������ �߻��߽��ϴ�.");
                    Debug.Log(task.Exception);
                }
            });
    }
    public void RespondToFriendRequest(string _userID,string _nickName,bool _addFriend)
    {
        Reference.Child("User").Child(User.UserId).Child("Push").Child("FriendRequest").Child(_userID).RemoveValueAsync().ContinueWithOnMainThread(
           (task) =>
           {
               if (task.IsCompleted)
               {
                   if(_addFriend)
                   {
                       Reference.Child("User").Child(User.UserId).Child("NickName").GetValueAsync().ContinueWithOnMainThread(
                           (task) =>
                           {
                               FriendRequestInfo friendinfo = new FriendRequestInfo(_userID, _nickName);
                               FriendRequestInfo myinfo = new FriendRequestInfo(User.UserId, task.Result.Value.ToString());
                               Reference.Child("User").Child(User.UserId).Child("Friend").Child(_userID).SetRawJsonValueAsync(JsonUtility.ToJson(friendinfo));
                               Reference.Child("User").Child(_userID).Child("Friend").Child(User.UserId).SetValueAsync(JsonUtility.ToJson(myinfo));

                           });
                   }
                }
               else
               {
                   UIManager.uiManager.OnErrorMessage("�˼����� ������ �߻��߽��ϴ�.");
                   Debug.Log(task.Exception);
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
