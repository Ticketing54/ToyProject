using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UIRequestSlot : UIUserSlot
{   
    public void OnClickAcceptButton()
    {
        AuthManager.Instance.RespondToFriendRequest(userId,true);
        Destroy(this.gameObject);
    }
    public void OnClickRefuseButton()
    {
        AuthManager.Instance.RespondToFriendRequest(userId,false);
        Destroy(this.gameObject);
    }
    
}
