using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UVLobbyMain : UIView
{
    [SerializeField]
    TextMeshProUGUI idText;

    private void OnEnable()
    {
        idText.text = AuthManager.Instance.UserData.NickName;
    }
    public void OnClickFriendButton()
    {

    }
}
