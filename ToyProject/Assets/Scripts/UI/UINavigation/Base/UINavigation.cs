using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UINavigation : MonoBehaviour
{
    [SerializeField]
    protected UIView rootView;
    protected Stack<UIView> history;
    protected UIView current;

    public virtual void Awake()
    {   
        history = new Stack<UIView>();
    }

    public virtual void RootShow() { StartCoroutine(CoRootShow()); }
    

    public virtual void Push(UIView _uiview)
    {
        if(current != null&& current.State != UIView.VisibleState.Appeared)
        {
            return;
        }
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
    protected virtual IEnumerator CoRootShow()
    {
        UIManager.uiManager.OnDontClick();
        rootView.gameObject.SetActive(true);
        if (current != null)
        {
            current.Hide();
            yield return new WaitWhile(() => current.State != UIView.VisibleState.Disappeared);
            current.gameObject.SetActive(false);
        }
        UIManager.uiManager.OFFDontClick();
        rootView.Show();
        history.Clear();
        current = rootView;
    }
    IEnumerator CoPushUIView(UIView _uiview)
    {
        UIManager.uiManager.OnDontClick();
        _uiview.gameObject.SetActive(true);
        if (current != null)
        {
            current.Hide();
            yield return new WaitWhile(() => current.State != UIView.VisibleState.Disappeared);
            current.gameObject.SetActive(false);
        }
        UIManager.uiManager.OFFDontClick();
        _uiview.Show();
        history.Push(current);
        current = _uiview;
    }
    IEnumerator CoPopUIView()
    {
        UIManager.uiManager.OnDontClick();
        yield return new WaitWhile(() => current.State != UIView.VisibleState.Disappeared);        
        current.gameObject.SetActive(false);
        UIManager.uiManager.OFFDontClick();
        UIView prevView = history.Pop();
        prevView.gameObject.SetActive(true);
        prevView.Show();
        current = prevView;
    }
    
}
