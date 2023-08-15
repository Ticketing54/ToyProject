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

    public override void OpenNavigation()
    {
        throw new NotImplementedException();
    }

    public override void CloseNavigation()
    {
        throw new NotImplementedException();
    }

    public override void ReturnToRoot()
    {
        throw new NotImplementedException();
    }

    public override void Push(UIView _uiview)
    {
        throw new NotImplementedException();
    }

    public override UIView Pop()
    {
        throw new NotImplementedException();
    }
}
