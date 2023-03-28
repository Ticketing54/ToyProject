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
                    Reference.Child("User").Child(User.UserId).GetValueAsync().ContinueWithOnMainThread(
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
        Reference.Child("User").Child(User.UserId).Child("Push").Child("FriendRequest").ValueChanged += HandleFriendRequestChanged;
        Reference.Child("User").Child(User.UserId).Child("Friend").ValueChanged += HandleFriend;

    }

    /// <summary>
    /// Action FriendChanged (UID,NickName)
    /// </summary>
    public Action<string,string> AFriendAdd { get; set; }
    public Action AFriendListClear { get; set; }
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

    public void UpdateFriendList()
    {
        Reference.Child("User").Child(User.UserId).Child("Friend").GetValueAsync().ContinueWithOnMainThread(
            (task) =>
            {
                if(task.IsCompleted)
                {
                    DataSnapshot friendsData = task.Result;
                    if(friendsData.Exists)
                    {
                        foreach(DataSnapshot friend in friendsData.Children)
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

    void HandleFriendRequestChanged(object sender, ValueChangedEventArgs e)
    {
        if (ACheckFriendRequests == null)
            return;

        if(e.DatabaseError != null)
        {
            Debug.LogError(e.DatabaseError.Message);
            return;
        }
        FindUser_UID(e.Snapshot.Key,ACheckFriendRequests);
        CheckFriendRequests();
    }
    /// <summary>
    /// UI활성화 시에만
    /// </summary>
    public Action<UserInfo> AUpdateFriendsList { get; set; }
    /// <summary>
    /// UI활성화시에만 (NickName,UID)
    /// </summary>
    public Action<string,string> AUpdateFindUserUI { get; set; }
    /// <summary>
    /// UI활성화시에만 (NickName,UID)
    /// </summary>
    public Action<string,string> ACheckFriendRequests { get; set; }
   
    public void CheckFriendRequests()
    {
        Reference.Child("User").Child(User.UserId).Child("Push").Child("FriendRequest").GetValueAsync().ContinueWithOnMainThread(
           (task) =>
           {
               if (ACheckFriendRequests == null)
                   return;

               if (task.IsFaulted)
               {
                   Debug.Log("없음");
               }
               else if (task.IsCompleted)
               {
                   if(task.Result.Exists)
                   {
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
    /// <summary>
    /// UID로 유저찾기  닉네임이 없는경우를 생각하기
    /// </summary>
    /// <param name="UID"></param>
    /// <param name="Action(UID,NickName)"></param>
    void FindUser_UID(string _uid,Action<string,string> _ui)
    {
        Reference.Child("User").Child(_uid).GetValueAsync().ContinueWithOnMainThread(
           (task) =>
           {
               if (task.IsFaulted)
               {
                   Debug.Log("없음");
               }
               else if (task.IsCompleted)
               {
                   if(task.Result.Exists)
                   {
                       DataSnapshot userdata = task.Result;
                       UserInfo userinfo = JsonUtility.FromJson<UserInfo>(userdata.GetRawJsonValue());
                       _ui(userinfo.UID, userinfo.NickName);
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
                    UIManager.uiManager.OnErrorMessage("알수없는 오류가 발생했습니다.");
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
                       Reference.Child("User").Child(User.UserId).Child("Friend").Child(_userID).SetValueAsync(true);
                       Reference.Child("User").Child(_userID).Child("Friend").Child(User.UserId).SetValueAsync(true);
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
