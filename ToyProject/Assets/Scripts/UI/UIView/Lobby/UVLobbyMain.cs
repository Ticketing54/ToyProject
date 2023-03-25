using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UVLobbyMain : UIView
{
    [SerializeField]
    TextMeshProUGUI idText;
    [SerializeField]
    GameObject friendRequestMarkUI;
    private void OnEnable()
    {
        AuthManager.Instance.AFriendRequestUI += FriendRequestMark;
    }
    private void OnDisable()
    {
        AuthManager.Instance.AFriendRequestUI -= FriendRequestMark;
    }
    void FriendRequestMark(string _nickName, string _UID) { friendRequestMarkUI.gameObject.SetActive(true); }

    public void OnClickFriendButton()
    {

    }
}
