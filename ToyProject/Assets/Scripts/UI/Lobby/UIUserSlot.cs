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
    public virtual void SetProfile(string _userName, string _userId)
    {
        nicName.text = _userName;
        userId = _userId;
    }
    public virtual void Clear()
    {
        nicName.text = "";
        userId = "";
    }

}
