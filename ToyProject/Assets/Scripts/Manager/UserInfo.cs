using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInfo : MonoBehaviour
{
    public string nickname;
    public string email;
    public bool playing;
    public UserInfo(string _nicName,string _email)
    {
        nickname = _nicName;
        email = _email;
        playing = false;
    }
    public UserInfo()
    {   
    }
}
