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
    public void Login(string _id,string _pw,Action _uiAction)
    {
        Auth.SignInWithEmailAndPasswordAsync(_id, _pw).ContinueWith(
            (task) =>
            {
                if(task.IsFaulted)
                {
                    _uiAction();
                }
                else if (task.IsCanceled)
                {
                    Debug.Log("�α��� ���");
                }
                else
                {
                    Debug.Log("�α��� ����");
                    // ���̵�
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
                if (task.IsFaulted)
                {
                    _uiAction();
                }
                else if (task.IsCanceled)
                {
                    UIManager.uiManager.OnErrorMessage("Regist ���� ���");
                }
                else
                {
                    Debug.Log("���� ����");
                    UIManager.uiManager.CurrentPop();
                }
            });
    }


}
