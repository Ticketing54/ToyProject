using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UVLobbyRoom : UIView
{
    [SerializeField]
    UIRoomUserSlot masterSlot;  // == user0
    [SerializeField]
    List<UIRoomUserSlot> guestSlot;
    [SerializeField]
    Button playerButton;


    private void OnEnable()
    {
        UIManager.uiManager.ARoomUpdate += SettingRoom;
    }
    private void OnDisable()
    {
        UIManager.uiManager.ARoomUpdate -= SettingRoom;
    }
    public void OnLeaveRoomButton() { LobbyManager.Instance.LeaveRoom(); }
    void SettingRoom(List<UserInfo> _userInfo)
    {
        int index = 1;
        masterSlot.SetProfile(_userInfo[0]);
        foreach(UIRoomUserSlot slot in guestSlot)
        {
            if(_userInfo.Count <=index)
            {
                slot.Clear();
            }
            else
            {
                slot.SetProfile(_userInfo[index++]);
            }
        }

        if(_userInfo[0].UID == AuthManager.Instance.User.UserId)
        {
            playerButton.gameObject.SetActive(true);
        }
        else
        {
            playerButton.gameObject.SetActive(false);
        }
    }
    public override void Show()
    {
        canvasGroup.alpha = 1;
        state = VisibleState.Appeared;
    }
    public override void Hide()
    {
        state = VisibleState.Disappeared;
    }
}
