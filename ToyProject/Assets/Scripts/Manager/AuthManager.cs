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
        yield return FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(
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
    public UserInfo UserData { get; private set; }
    public DatabaseReference Reference { get; private set; }

    public Action OpenNickNameUI;
    
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

                                if(UserData.nickname == "")
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
                    UserInfo temp = new ();                    
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
    /// ������ ã�� �� �� �ൿ
    /// </summary>
    /// <param name="NickName"></param>
    /// <param name="Action(UserID,UserInfo)"></param>
    public void FindUser(string _nickName, Action<string,UserInfo> _action)
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
                        _action(child.Key.ToString(), new UserInfo(_nickName));
                    }
                }
            });
    }
    /// <summary>
    ///  ������ ��ϵ� ģ�� ����Ʈ �� Action<UserID, UserInfo>
    /// </summary>
    /// <param name="Action(UserID,UserInfo)"></param>
    public void FindFriends(Action<string,UserInfo> _action)
    {

    }
}
