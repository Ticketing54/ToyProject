using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UVLobbyRoom : UIView
{
    [SerializeField]
    UIRoomUserSlot master;  // == user0
    [SerializeField]
    UIRoomUserSlot user1;
    [SerializeField]
    UIRoomUserSlot user2;
    [SerializeField]
    UIRoomUserSlot user3;
    [SerializeField]
    Button playerButton;


    private void OnEnable()
    {
        AuthManager.Instance.AUpdateRoom += SettingSlot;
    }
    private void OnDisable()
    {
        AuthManager.Instance.AUpdateRoom -= SettingSlot;
    }
    void SettingSlot(FBRoomData _data,bool _isMaster)
    {
        playerButton.gameObject.SetActive(_isMaster);

        if (_data.Master != "")
        {
            AuthManager.Instance.FindUser_UID(_data.Master, master.SetProfile);
        }
        else
        {
            master.Clear();
        }

        if (_data.User1 != "")
        {
            AuthManager.Instance.FindUser_UID(_data.User1, user1.SetProfile);
        }
        else
        {
            user1.Clear();
        }
        if (_data.User2 != "")
        {
            AuthManager.Instance.FindUser_UID(_data.User2, user2.SetProfile);
        }
        else
        {
            user2.Clear();
        }

        if (_data.User3 != "")
        {
            AuthManager.Instance.FindUser_UID(_data.User3, user3.SetProfile);
        }
        else
        {
            user3.Clear();
        }
    }

}
