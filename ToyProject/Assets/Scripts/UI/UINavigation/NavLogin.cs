using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavLogin : UINavigation
{
    [SerializeField]
    UIView regist;
    
    public override void Awake()
    {
        base.Awake();
    }
    public override UIView Pop()
    {
        if (history.Count == 1&&current.State != UIView.VisibleState.Disappeared)
            return null;

        return null;
      // 다시 생각해볼것  
    }
    public void OnClickRegistButton()
    {

    }
    public void OnClickRegistExitButton()
    {

    }
    public override void Push(UIView _uiview)
    {
        
    }    
}
