using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIRoomUserSolt : UIUserSlot
{
    [SerializeField]
    Button inviteButton;

    public override void SetProfile(UserInfo _userinfo)
    {
        inviteButton.gameObject.SetActive(false);
        base.SetProfile(_userinfo);
    }
    public override void Clear()
    {
        inviteButton.gameObject.SetActive(true);
        base.Clear();
    }
}
