using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UserInfo 
{
    public string UserId;
    public string NickName;
    public List<string> Friends;
    public UserInfo(string _userId ,string _nickName="",List<string> _friends = null)
    {
        UserId = _userId;
        NickName = _nickName;
        Friends = _friends;
    }
}
