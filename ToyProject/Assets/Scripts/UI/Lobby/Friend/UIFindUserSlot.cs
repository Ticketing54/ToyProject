using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using UnityEngine.UI;
public class UIFindUserSlot : UIUserSlot
{   

    [SerializeField]
    Button AddButton;

    public override void SetProfile(UserInfo _userinfo)
    {
        base.SetProfile(_userinfo);
        AddButton.gameObject.SetActive(true);
    }
    public override void Clear()
    {
        base.Clear();
        AddButton.gameObject.SetActive(false);
    }
    public void OnClickAddFriendButton()
    {
        AddButton.gameObject.SetActive(false);
        AuthManager.Instance.SendFriendRequest(userId);
    }

}
