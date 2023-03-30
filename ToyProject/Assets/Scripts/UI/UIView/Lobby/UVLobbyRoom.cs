using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UVLobbyRoom : UIView
{
    [SerializeField]
    UIRoomUserSolt master;  // == user0
    [SerializeField]
    UIRoomUserSolt user1;
    [SerializeField]
    UIRoomUserSolt user2;
    [SerializeField]
    UIRoomUserSolt user3;

    Button playerButton;


    private void Awake()
    {
        
    }

    void SettingSlot(int _index, UserInfo _userinfo)
    {
        UIRoomUserSolt controlslot;
        switch(_index)
        {
            case 0:
                controlslot = master;
                break;
            case 1:
                controlslot = user1;
                break;
            case 2:
                controlslot = user2;
                break;
            case 3:
                controlslot = user3;
                break;
            default:
                controlslot = null;
                Debug.Log("UVLobbyRoom :: SettingSlot Number is Error");
                break;
        }

        if(controlslot != null)
        {
            controlslot.SetProfile(_userinfo);
        }
    }

}
