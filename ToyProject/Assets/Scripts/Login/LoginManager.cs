using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class LoginManager : MonoBehaviour
{
    public static LoginManager loginManager;
    const string URL = "https://docs.google.com/spreadsheets/d/1r84Xh15BPtjMQk5m38sNB5co3IJSqDIKUCTtDHB8Nkc/export?format=tsv";

    string id, password;    
    
    private void Awake()
    {
        if(loginManager == null)
        {
            loginManager = this;
        }
        else
        {
            Destroy(this);
        }
    }    
   
    public void Login(string _id,string _pw)
    {
        id = _id;
        password = _pw;
            
    }
   
    public void Regist()
    {
        
    }
}
