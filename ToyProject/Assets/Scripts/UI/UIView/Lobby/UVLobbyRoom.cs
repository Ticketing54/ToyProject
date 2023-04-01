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
        
    }
    private void OnDisable()
    {
        
    }

}
