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
        root.gameObject.SetActive(true);
        root.Show();
        current = root;
    }

    public virtual void Push(UIView _uiview)
    {
        if(current.State != UIView.VisibleState.Appeared)
        {
            return;
        }
        current.Hide();
        StartCoroutine(CoPushUIView(_uiview));        
    }        
    public virtual UIView Pop()
    {
        if (current.State != UIView.VisibleState.Appeared || history.Count == 0)
        {
            Debug.Log("History is Empty or Not Ready");
            return null;
        }        
        current.Hide();
        UIView popUIView = current;
        StartCoroutine(CoPopUIView());

        return popUIView;
    }

    IEnumerator CoPushUIView(UIView _uiview)
    {
        yield return new WaitWhile(() => current.State != UIView.VisibleState.Disappeared);
        current.gameObject.SetActive(false);
        _uiview.gameObject.SetActive(true);
        _uiview.Show();
        history.Push(current);
        current = _uiview;
    }
    IEnumerator CoPopUIView()
    {
        yield return new WaitWhile(() => current.State != UIView.VisibleState.Disappeared);        
        current.gameObject.SetActive(false);
        UIView prevView = history.Pop();
        prevView.gameObject.SetActive(true);
        prevView.Show();
        current = prevView;
    }
    
}
