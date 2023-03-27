using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UserInfo 
{
    public string UID;
    public string NickName;
    public UserInfo()
    {
        UID = "";
        NickName = "";
        
    }
    public UserInfo(string _userId,string _nickName)
    {
        UID = _userId;
        NickName = _nickName;
        
    }
}