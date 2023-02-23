using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
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

    public void Init()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(
            (task) =>
            {
                if(task.Result == DependencyStatus.Available)
                {
                    Auth = FirebaseAuth.DefaultInstance;
                    App = FirebaseApp.DefaultInstance;
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

        Auth.SignInWithEmailAndPasswordAsync(_id, _pw).ContinueWithOnMainThread(
            (task) =>
            {
                if(task.IsCanceled)
                {
                    Debug.Log("Login Canceled");
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
                                    
                                    UIManager.uiManager.OnErrorMessage("�н����带 �ٽ��Է��� �ּ���");
                                    
                                }
                                break;
                            case AuthError.WrongPassword:
                                {
                                    UIManager.uiManager.OnErrorMessage("�н����带 �ٽ��Է��� �ּ���");
                                    
                                }
                                break;
                            default:
                                Debug.Log(error);
                                break;
                        }
                    }
                }
                else
                {
                    User = task.Result;
                }
            });


       
       
    }
    /// <summary>
    /// Regist : ID�� Password, ��������� UIǥ���Լ�
    /// </summary>
    /// <param name="ID"></param>
    /// <param name="PW"></param>
    /// <param name="UIAction"></param>
    public void Regist(string _id,string _pw,Action _uiAction)
    {
        Auth.CreateUserWithEmailAndPasswordAsync(_id, _pw).ContinueWith(
            (task) =>
            {
                
                if (task.IsFaulted || task.IsCanceled)
                {
                    _uiAction();
                }
                else
                {
                    Debug.Log("���� ����");
                    UIManager.uiManager.CurrentPop();
                }
            });
    }


}
