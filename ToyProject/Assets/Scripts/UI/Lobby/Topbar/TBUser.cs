using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TBUser : TopBarIcon
{
    [SerializeField]
    GameObject userUI;
    private void OnEnable()
    {
        UIManager.uiManager.AMarkingFriendButton += Mark;
    }
    private void OnDisable()
    {
        UIManager.uiManager.AMarkingFriendButton -= Mark;
    }
    public override void OnClickButton()
    {
        EraseMark();
        userUI.gameObject.SetActive(true);
    }
}
