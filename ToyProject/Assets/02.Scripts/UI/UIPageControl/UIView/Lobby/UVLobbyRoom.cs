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
    [SerializeField]
    UIWaitPlayGame count;

    private void OnEnable()
    {
        UIManager.Instance.ARoomUpdate += SettingRoom;
        UIManager.Instance.AOpenCounter += OpenCounter;
    }
    private void OnDisable()
    {
        UIManager.Instance.ARoomUpdate -= SettingRoom;
        UIManager.Instance.AOpenCounter -= OpenCounter;
    }

    void OpenCounter() { count.gameObject.SetActive(true); }
    public void OnClickPlayerButton() { LobbyManager.Instance.StartGame(); }
    public void OnLeaveRoomButton() { LobbyManager.Instance.LeaveRoom(); }
    void SettingRoom(Queue<UserInfo> _userInfo)
    {
        UserInfo masterUser = _userInfo.Dequeue();
        masterSlot.SetProfile(masterUser);
        if (masterUser.UID == AuthManager.Instance.User.UserId)
        {
            playerButton.gameObject.SetActive(true);
        }
        else
        {
            playerButton.gameObject.SetActive(false);
        }
        
        foreach(UIRoomUserSlot slot in guestSlot)
        {
            UserInfo user = null;
            if (_userInfo.Count != 0)
            {
                user = _userInfo.Dequeue();
            }

            if(user == null)
            {
                slot.Clear();
            }
            else
            {
                slot.SetProfile(user);
            }
        }
    }
    
}
