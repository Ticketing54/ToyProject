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

    public Action OpenNickNameUI { get; set; }

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
                    Reference.Child("User").Child(User.UserId).Child("NickName").GetValueAsync().ContinueWithOnMainThread(
                        (task) =>
                        { 
                            if(task.IsFaulted || task.IsCanceled)
                            {
                                GameManager.Instance.ChangeState(GameState.Login);
                                UIManager.uiManager.OnErrorMessage("로그인 정보가 존재하지 않습니다.");
                                return;
                            }
                            // 이벤트 리스너 
                            AddEventListner();
                            DataSnapshot datasnapshot = task.Result;

                            if(datasnapshot.Exists)             // 닉네임 설정이 되있는경우
                            {
                                GameManager.Instance.ChangeState(GameState.Lobby);
                            }
                            else
                            {
                                if(OpenNickNameUI != null)
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
                    Reference.Child("User").Child(task.Result.UserId).Child("Push").Child("FriendRequest").Child("Empty").SetValueAsync(true);
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
                    UIManager.uiManager.OnErrorMessage("NicName 등록 오류");
                }
            });
    }

    public void AddEventListner()
    {
        Reference.Child("User").Child(User.UserId).Child("Push").Child("FriendRequest").ChildAdded += HandleFriendRequestChanged;
    }
    /// <summary>
    /// Action<UserNickName,UserID>
    /// </summary>
    public Action<string, string> AFriendRequestUI { get; set; }
    void HandleFriendRequestChanged(object sender, ChildChangedEventArgs args)
    {
        if (AFriendRequestUI == null)
            return;

        if(args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        AFriendRequestUI(args.Snapshot.Key, args.Snapshot.Key);
    }
    /// <summary>
    /// UI활성화 시에만
    /// </summary>
    public Action<UserInfo> AUpdateFriendsList { get; set; }
    /// <summary>
    /// UI활성화시에만 (NickName,UID)
    /// </summary>
    public Action<string,string> AUpdateFindUserUI { get; set; }
    
    public void CheckFriendRequests(Action<string,string> _ui)
    {
        Reference.Child("User").Child("Push").Child("FriendRequest").OrderByChild("Request").GetValueAsync().ContinueWithOnMainThread(
           (task) =>
           {
               if (task.IsFaulted)
               {
                   Debug.Log("없음");
               }
               else if (task.IsCompleted)
               {
                   DataSnapshot friends = task.Result;
                   foreach(DataSnapshot datasnapshot in friends.Children)
                   {
                       if(datasnapshot.Value.ToString() == "Request")
                       {
                           FindUserList_UserID(datasnapshot.Key, _ui);
                       }
                       
                   }
               }
           });
    }

    void FindUserList_UserID(string _userId , Action<string,string> _ui)
    {
        Reference.Child("User").Child("User").Child(_userId).Child("NickName").GetValueAsync().ContinueWithOnMainThread(
           (task) =>
           {
               if (task.IsFaulted)
               {
                   Debug.Log("없음");
               }
               else if (task.IsCompleted)
               {
                   _ui(_userId, task.Result.ToString());
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
                   Debug.Log("없음");
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
    public void FriendRequest(string _targetUID)
    {
        Reference.Child("User").Child(_targetUID).Child("Push").Child("FriendRequest").Child(User.UserId).SetValueAsync("Request").ContinueWithOnMainThread(
            (task) =>
            {
                if (task.IsCompleted)
                {
                    // 좀더 추가 할 예정                    
                }
                else
                {
                    UIManager.uiManager.OnErrorMessage("알수없는 오류가 발생했습니다.");
                    Debug.Log(task.Exception);
                }
            });
    }
    public void RespondToFriendRequest(string _userID,bool _addFriend)
    {
        Reference.Child("User").Child(User.UserId).Child("Push").Child("FriendRequest").Child(_userID).RemoveValueAsync().ContinueWithOnMainThread(
           (task) =>
           {
               if (task.IsCompleted)
               {
                   if(_addFriend)
                   {
                       Reference.Child("User").Child(User.UserId).Child("Friend").Child(_userID).SetValueAsync("Friend");
                   }
                }
               else
               {
                   UIManager.uiManager.OnErrorMessage("알수없는 오류가 발생했습니다.");
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
