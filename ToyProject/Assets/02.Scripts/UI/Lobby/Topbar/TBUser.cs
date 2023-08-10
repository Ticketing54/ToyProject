using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TBUser : TopBarIcon
{
    [SerializeField]
    GameObject userUI;
    private void OnEnable()
    {
        UIManager.Instance.AMarkingFriendButton += Mark;
    }
    private void OnDisable()
    {
        UIManager.Instance.AMarkingFriendButton -= Mark;
    }
    public override void OnClickButton()
    {
        EraseMark();
        userUI.gameObject.SetActive(true);
    }
}
