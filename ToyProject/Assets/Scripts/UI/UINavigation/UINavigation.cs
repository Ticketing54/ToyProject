using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UINavigation : MonoBehaviour
{
    protected Stack<UIView> history;    
    public virtual void Awake()
    {
        history = new Stack<UIView>();
        
    }
    public abstract void Push(UIView _uiview);
    public abstract UIView Pop();
}
