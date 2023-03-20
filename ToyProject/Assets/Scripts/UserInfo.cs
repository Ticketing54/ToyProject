using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInfo 
{
    public string nickname;
    public bool playing;
    public UserInfo(string _nicName="")
    {
        nickname = _nicName;
        playing = false;
    }
    public UserInfo()
    {
        playing = false;
    }
}
