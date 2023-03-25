using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UserInfo 
{
    public string UserId;
    public string NickName;
    public string RecentConnectTime;
    public UserInfo()
    {
        UserId = "";
        NickName = "";
        RecentConnectTime = "";
    }
    public UserInfo(string _userId,string _nickName="",string _recentConnectTime = "")
    {
        UserId = _userId;
        NickName = _nickName;
        RecentConnectTime = _recentConnectTime;
    }
}


