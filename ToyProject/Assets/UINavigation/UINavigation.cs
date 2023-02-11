using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
public class UINavigation : MonoBehaviour
{   
    public static UINavigation uiNav;
    private UIView current;
    Stack<UIView> history;

    private void Awake()
    {
        if (uiNav == null)
        {
            uiNav = this;
            DontDestroyOnLoad(this.gameObject);            
            history = new Stack<UIView>();
        }
        else
        {
            Destroy(uiNav);
        }
    }    
    
    #region ErrorMessage
    [SerializeField]
    ErrorMessage errorMessage;
    public void OnDontTouch()
    {
        errorMessage.gameObject.SetActive(true);
        errorMessage.transform.SetParent(current.transform);
        errorMessage.transform.localPosition = Vector3.zero;
    }
    public void OnErrorMessage(string _msg)
    {
        errorMessage.gameObject.SetActive(true);
        errorMessage.transform.SetParent(current.transform);
        errorMessage.transform.localPosition = Vector3.zero;        
        errorMessage.SetErrorMessage(_msg);
    }
    public void OffErrorMessage()
    {
        errorMessage.gameObject.SetActive(false);
    }
    #endregion
   
    public UIView Push(UIView _pushUIView)
    {
        StartCoroutine(CoPush(_pushUIView));
        return null;
    }
    IEnumerator CoPush(UIView _pushUIView)
    {
        OnDontTouch();       
        yield return _pushUIView.Show();
        OffErrorMessage();
    }
    public UIView Pop()
    {
        // stack 에서 지우기
        return current;
    }
    

}
