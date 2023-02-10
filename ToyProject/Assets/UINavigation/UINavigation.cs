using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
public class UINavigation : MonoBehaviour
{   
    public static UINavigation uiNav;
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
  
    #region DontTouch
    static Image dontTouchScreen;
    public static void DontTouchScreenOn(UIView _current)
    {
        dontTouchScreen.gameObject.SetActive(true);
        dontTouchScreen.transform.SetParent(_current.gameObject.transform);
        dontTouchScreen.transform.localPosition = Vector3.zero;
    }
    public static void DontTouchScreenOff()
    {
        dontTouchScreen.transform.SetParent(null);
        dontTouchScreen.gameObject.SetActive(false);
    }
    #endregion

    private void Awake()
    {
        if(uiNav == null)
        {
            uiNav = this;
            DontDestroyOnLoad(this.gameObject);
            dontTouchScreen = GameObject.FindGameObjectWithTag("DontTouch").GetComponent<Image>();
            uiViewDic = new Dictionary<string, UIView>();
            history = new Stack<UIView>();
        }
        else
        {
            Destroy(uiNav);            
        }
    }


    
    public UIView Push(string _uiviewName)
    {
        if(uiViewDic.ContainsKey(_uiviewName))
        {
            UIView cUIView = uiViewDic[_uiviewName];
            cUIView.Show();
            history .Push(cUIView);
            if(current != null)
            {
                current.Hide();
            }
            current = cUIView;
        }
        else
        {
            Debug.LogError("Wrong UIViewName");
            return null;
        }
        
        return null;
    }
    public UIView Pop()
    {
        // stack 에서 지우기
        return current;
    }
    

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.K))
        {
            Push("login");
        }
        
    }
}
