using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class UIUserSlot : MonoBehaviour
{   
    [SerializeField]
    protected TextMeshProUGUI nickName;
    protected string userId;
    protected string userName;
    public virtual void SetProfile(UserInfo _userinfo)
    {
        userId = _userinfo.UID;
        nickName.text = _userinfo.NickName;
    }
    public virtual void Clear()
    {
        nickName.text = "";
        userId = "";
    }

}
