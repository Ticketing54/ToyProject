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
        playerButton.interactable =_isMaster;

        if (_data.Master != "")
        {
            
        }
        else
        {
            master.Clear();
        }

        if (_data.User1 != null)
        {
            
        }
        else
        {
            user1.Clear();
        }
        if (_data.User2 != null)
        {
            
        }
        else
        {
            user2.Clear();
        }
        if (_data.User3 != null)
        {
            
        }
        else
        {
            user3.Clear();
        }

    }
}
