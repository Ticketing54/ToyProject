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
                    Reference.Child("ChatRoom").ValueChanged += HandleMessageChanged;
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
    public UserInfo UserData { get; private set; }
    public DatabaseReference Reference { get; private set; }

    public Action OpenNickNameUI;

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
                            if(task.IsCompleted)
                            {
                                DataSnapshot datasnapshot = task.Result;
                                string temp = datasnapshot.GetRawJsonValue();
                                UserData = JsonUtility.FromJson<UserInfo>(temp);

                                if(UserData.NickName == "")
                                {   
                                    OpenNickNameUI();
                                    UIManager.uiManager.OFFDontClick();
                                }
                                else
                                {
                                    GameManager.Instance.ChangeState(GameState.Lobby);
                                }
                            }
                            else
                            {
                                // ���� FirebaseDataBase�� �Է��ϰ� ����� ���̵� ���� �� ��
                                //Reference.Child("Error").Child("AuthID").SetValueAsync(User.UserId);
                                User = null;
                                GameManager.Instance.ChangeState(GameState.Login);
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
                    UIManager.uiManager.OFFDontClick();
                    UIManager.uiManager.CurrentPop();
                    UserInfo temp = new (task.Result.UserId);
                    string jsondata = JsonUtility.ToJson(temp);
                    Reference.Child("User").Child(task.Result.UserId).SetRawJsonValueAsync(jsondata);
                }
            });
    }
    public void SetNickName(string _nicName)
    {
        UIManager.uiManager.OnDontClick();

        Reference.Child("User").Child(User.UserId).Child("nickname").SetValueAsync(_nicName).ContinueWithOnMainThread(
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
    /// <summary>
    /// UIȰ��ȭ�ÿ��� [ģ��ã��]
    /// </summary>
    public Action<UserInfo> AUpdateFriendsList
    {
        get => aUpdateFriendsList;
        set
        {
            if(aUpdateFriendsList == null)
            {
                Debug.Log("(Action) updateFriendList is Null");
                return;
            }
            else
            {
                aUpdateFriendsList = value;
            }
        }
    }
    /// <summary>
    /// UIȰ��ȭ�ÿ��� [ģ�����]
    /// </summary>
    public Action<UserInfo> AUpdateFindUser
    {
        get => aUpdateFindUser;
        set
        {
            if (aUpdateFindUser == null)
            {
                Debug.Log("(Action) updateFriendList is Null");
                return;
            }
            else
            {
                aUpdateFindUser = value;
            }
        }
    }
    private Action<UserInfo> aUpdateFriendsList;
    private Action<UserInfo> aUpdateFindUser;
    public void UpdateFindUser(string _userID) { FindUser_UserID(_userID, AUpdateFindUser); }
    public void UpdateFriendsList(string _nickName) { FindUser_UserID(_nickName, AUpdateFriendsList); }
    

    /// <summary>
    /// [NickName] �� ����Ͽ� ������ ã�� �� �� �ൿ
    /// </summary>
    /// <param name="NickName"></param>
    /// <param name="Action(UserID,UserInfo)"></param>
    void FindUser_NickName(string _nickName, Action<UserInfo> _action)
    {   
        Reference.Child("User").OrderByChild("nickname").EqualTo(_nickName).GetValueAsync().ContinueWithOnMainThread(
            (task) =>
            {
                
                if(task.IsFaulted)
                {
                    Debug.Log("����");
                }
                else if(task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    foreach(DataSnapshot child in snapshot.Children)
                    {
                        string temp = child.GetRawJsonValue();
                        UserInfo newUser = JsonUtility.FromJson<UserInfo>(temp);
                        _action(newUser);
                    }
                }
            });
    }
    /// <summary>
    /// [UserID] �� ����Ͽ� ������ ã�� �� �� �ൿ
    /// </summary>
    /// <param name="NickName"></param>
    /// <param name="Action(UserID,UserInfo)"></param>
    void FindUser_UserID(string _userId, Action<UserInfo> _action)
    {
        
        Reference.Child("User").Child(_userId).GetValueAsync().ContinueWithOnMainThread(
            (task) =>
            {
                if (task.IsFaulted)
                {
                    Debug.Log("����");
                }
                else if (task.IsCompleted)
                {
                    string temp = task.Result.GetRawJsonValue();
                    UserInfo newUser = JsonUtility.FromJson<UserInfo>(temp);
                    _action(newUser);
                }
            });
    }

    public void AddFriends()
    {

    }
    public void RemoveFriend()
    {

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
    public void SendMessage(string _receiver, string _message)
    {

    }
    private void HandleMessageChanged(object _sender,ValueChangedEventArgs _arg)
    {

    }
    #endregion
}
