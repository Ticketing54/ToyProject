using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIUserSlot : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI nicName;
    protected string userId;
    protected string userName;
    public virtual void SetProfile(UserInfo _userinfo)
    {
        userId = _userinfo.UID;
        nicName.text = _userinfo.NickName;
    }
    public virtual void Clear()
    {
        nicName.text = "";
        userId = "";
    }

}
