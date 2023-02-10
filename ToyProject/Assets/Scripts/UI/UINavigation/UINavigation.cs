using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UINavigation : MonoBehaviour
{
    private UIView current;
    Stack<UIView> history;
    public UIView Push(string _uiviewName)
    {
        UIView uiView = UIView.Get(_uiviewName);
        history.Push(uiView);
        return uiView;
    }
    public UIView Pop()
    {
        // stack ���� �����
        return current;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
