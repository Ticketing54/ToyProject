using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavLogin : UINavigation
{
    [SerializeField]
    UIView login;
    [SerializeField]
    UIView regist;

    public override void Awake()
    {
        base.Awake();
        Push(login);

    }
    public override UIView Pop()
    {
        throw new System.NotImplementedException();
    }

    public override void Push(UIView _uiview)
    {
        throw new System.NotImplementedException();
    }
}
