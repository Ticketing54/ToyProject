using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NavSign : UINavigation
{
    [SerializeField]
    UIView regist;
    
    public override void Awake()
    {
        base.Awake();            
    }

    
    public void OnClickRegistButton() { Push(regist); }    
    public void OnClickPopButton() { Pop(); }
}
