using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UIFriendSlot : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI nicName;
    string userId;
    string userName;
    public virtual void SetProfile(string _userId,string _userName)
    {
        userId = _userId;
        nicName.text = _userName;
    }
    public virtual void Clear()
    {   
        nicName.text = "";
        userId = "";
    }
}
