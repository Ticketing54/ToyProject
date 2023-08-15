using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UINavigation : MonoBehaviour
{
    [SerializeField] protected UIView rootView;

    protected Stack<UIView> history = new Stack<UIView>();
    protected UIView current;
    public abstract void OpenNavigation();
    public abstract void CloseNavigation();
    public abstract void ReturnToRoot();
    public abstract void Push(UIView _uiview);
    public abstract UIView Pop();
}
