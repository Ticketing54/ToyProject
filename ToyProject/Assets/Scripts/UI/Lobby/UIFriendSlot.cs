using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UIFriendSlot : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI nicName;
    public UserInfo UserInfo { get; private set; }
    public virtual void SetProfile(string _userId,UserInfo _userinfo)
    {  
        UserInfo = _userinfo;
        nicName.text = _userinfo.NickName;
    }
    public virtual void Clear()
    {   
        nicName.text = "";
        UserInfo = null;
    }
}
