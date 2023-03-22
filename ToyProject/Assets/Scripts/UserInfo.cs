using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UserInfo 
{
    public string UserId;
    public string NickName;
    public List<string> Friends;
    public List<string> AddFriendSendList;
    public List<string> AddFriendReceiveList;
}
