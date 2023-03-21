using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UIFriend : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI nicName;

    public string UserId { get; private set; }
    public UserInfo UserInfo { get; private set; }
    public void SetProfile(string _userId,UserInfo _userinfo)
    {
        UserId = _userId;
        UserInfo = _userinfo;
        nicName.text = _userinfo.nickname;
    }
    public void Clear()
    {
        nicName.text = "";
        UserId = "";
        UserInfo = null;
    }
}
