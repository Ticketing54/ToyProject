using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInviteFriendSlot : UIUserSlot
{
    [SerializeField]
    Button sendButton;
    public override void SetProfile(UserInfo _userinfo)
    {
        base.SetProfile(_userinfo);
        sendButton.gameObject.SetActive(true);
    }
    public void OnClickSendInviteRequest()
    {
        sendButton.gameObject.SetActive(false);

    }
}
