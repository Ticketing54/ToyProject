using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIFindUserSlot : UIUserSlot
{
    [SerializeField]
    Button AddButton;

    public override void SetProfile(string _userId, string _userNickname)
    {
        base.SetProfile(_userId, _userNickname);
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
        AuthManager.Instance.FriendRequest(userId);
    }

}
