using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInvitationSlot : UIUserSlot
{
    public string RoomName { get; set; }
    
    public void OnClickAcceptButton()
    {
        LobbyManager.Instance.JoinRoom(RoomName);
        Destroy(this.gameObject);
    }
    public void OnClickRefuseButton()
    {   
        Destroy(this.gameObject);
    }
}
