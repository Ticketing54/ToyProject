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
    public UserInfo(string _userId,string _nickName,string _recentConnectTime)
    {
        UserId = _userId;
        NickName = _nickName;
        RecentConnectTime = _recentConnectTime;
    }
}

[System.Serializable]
public class FriendRequestInfo
{
    public string UID;
    public string NickName;
    
    public FriendRequestInfo()
    {
        UID = "";
        NickName = "";
        
    }
    public FriendRequestInfo(string _userId, string _nickName = "")
    {
        UID = _userId;
        NickName = _nickName;        
    }
}


