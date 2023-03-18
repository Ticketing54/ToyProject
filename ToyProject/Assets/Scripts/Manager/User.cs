using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User : MonoBehaviour
{
    public string nickname;
    public string email;
    public bool playing;
    public User(string _nicName,string _email)
    {
        nickname = _nicName;
        email = _email;
        playing = false;
    }
    public User()
    {   
    }
}
