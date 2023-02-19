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

    public  void Init()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread((task) =>
        {
            DependencyStatus state = task.Result;

            if( state != DependencyStatus.Available)
            {
                Debug.LogError("FireBase�� ��������ʾҽ��ϴ�.");
                return;
            }
            //App = FirebaseApp.DefaultInstance;
            Auth = FirebaseAuth.DefaultInstance;
            //Auth.StateChanged += OnChanged;
        });
        
        

    }

    private void OnChanged(object sender, EventArgs e)
    {
        if(Auth.CurrentUser != User)
        {
            bool signed = (Auth.CurrentUser != User && Auth.CurrentUser != null);

            if(!signed && User != null)
            {
                // �α� �ƿ�
            }
            User = Auth.CurrentUser;

            if(signed)
            {
                // �α���
            }

        }
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
    public void Login(string _id,string _pw,Action<string>_uiAction)
    {
        Auth.SignInWithEmailAndPasswordAsync(_id, _pw).ContinueWith((task) =>
         {
             if (task.IsFaulted)
             {
                 Debug.Log("IsFaulted");
                 Debug.Log(task.Exception);

             }
             else if (task.IsCanceled) 
             {
                 Debug.Log("IsCanceled");
                 Debug.Log(task.Exception);
             }
             else
             {
                 Debug.Log("�α��� ����");
             }
         });
         
    }
    /// <summary>
    /// Regist : ID�� Password, ��������� UIǥ���Լ�
    /// </summary>
    /// <param name="ID"></param>
    /// <param name="PW"></param>
    /// <param name="UIAction"></param>
    public void Regist(string _id,string _pw,Action<string> _uiAction)
    {
        Auth.CreateUserWithEmailAndPasswordAsync(_id, _pw).ContinueWith((task) =>
        {
            
        });
    }


}
