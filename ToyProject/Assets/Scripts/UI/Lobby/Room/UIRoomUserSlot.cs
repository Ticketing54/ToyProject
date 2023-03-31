using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIRoomUserSlot : UIUserSlot
{
    [SerializeField]
    Button inviteButton;

    public override void SetProfile(UserInfo _userinfo)
    {
        bool isEmpty = (_userinfo.NickName == "") ? true : false;
        base.SetProfile(_userinfo);
        inviteButton.gameObject.SetActive(isEmpty);
        nickName.gameObject.SetActive(!isEmpty);
    }
    public override void Clear()
    {
        inviteButton.gameObject.SetActive(true);
        nickName.gameObject.SetActive(false);
        base.Clear();
    }
}
