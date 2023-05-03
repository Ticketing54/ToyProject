using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
public class UIFriendSlot : UIUserSlot
{
    [SerializeField]
    Image connectImage;

    public override void SetProfile(UserInfo _userinfo)
    {
        base.SetProfile(_userinfo);
        if (_userinfo.Connect)
            connectImage.color = Color.green;
        else
            connectImage.color = Color.gray;

    }
    public void OnClickFriendSlot()
    {

    }
}
