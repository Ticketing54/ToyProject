using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
public class UINavigation : MonoBehaviour
{   
    public static UINavigation uiNav;
    private void Awake()
    {
        if (uiNav == null)
        {
            uiNav = this;
            DontDestroyOnLoad(this.gameObject);
            uiViewDic = new Dictionary<string, UIView>();
            history = new Stack<UIView>();
        }
        else
        {
            Destroy(uiNav);
        }
    }

    public Dictionary<string, UIView> uiViewDic;
    private UIView current;
    Stack<UIView> history;

    public void PushUIViewDic(string _uiViewName,UIView _uiView)
    {
        if(!uiViewDic.ContainsKey(_uiViewName))
        {
            uiViewDic.Add(_uiViewName, _uiView);
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

    public void ChangeRoot(UIView _uiView)
    {

    }
    
    public UIView Push(UIView _pushUIView)
    {
        StartCoroutine(CoPush(_pushUIView));
        return null;
    }
    IEnumerator CoPush(UIView _pushUIView)
    {
        OnDontTouch();
        if(current != null)
        {
            yield return current.Hide();
        }
        yield return _pushUIView.Show();
        OffErrorMessage();
    }
    public UIView Pop()
    {
        // stack 에서 지우기
        return current;
    }
    

}
