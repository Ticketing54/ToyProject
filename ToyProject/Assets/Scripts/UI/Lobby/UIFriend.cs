using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UIFriend : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI nicName;
    public UserInfo UserInfo { get; private set; }
    public void SetProfile(string _userId,UserInfo _userinfo)
    {
        
        UserInfo = _userinfo;
        nicName.text = _userinfo.NickName;
    }
    public void Clear()
    {   
        nicName.text = "";
        UserInfo = null;
    }
}
