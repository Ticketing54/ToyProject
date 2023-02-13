using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UINavigation : MonoBehaviour
{
    protected Stack<UIView> history;
    protected UIView current;
    [SerializeField]
    UIView root;

    public virtual void Awake()
    {
        history = new Stack<UIView>();
        history.Push(root);
        root.Show();
    }
    public abstract void Push(UIView _uiview);
    public abstract UIView Pop();    
}
