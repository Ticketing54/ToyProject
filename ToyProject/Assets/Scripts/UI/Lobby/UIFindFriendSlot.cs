using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIFindFriendSlot : UIFriendSlot
{
    [SerializeField]
    Button AddButton;

    public override void SetProfile(string _userId, UserInfo _userinfo)
    {
        base.SetProfile(_userId, _userinfo);
    }

    public void SetButtonActive() { AddButton.gameObject.SetActive(true); }
    
    public override void Clear()
    {
        base.Clear();
        AddButton.gameObject.SetActive(false);
    }
    public void OnClickAddFriendButton()
    {
        //
    }

}
