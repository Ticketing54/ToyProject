using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NavSign : UINavigation
{   
    [SerializeField]
    UIView login;
    [SerializeField]
    UIView regist;
    [SerializeField]
    UIView nickNameSetting;

    public void OnClickRegistButton() { Push(regist); }    
    public void OnClickPopButton() { Pop(); }
}
