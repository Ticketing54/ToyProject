using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIUserSlot : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI nicName;
    protected string userId;
    protected string userName;
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
