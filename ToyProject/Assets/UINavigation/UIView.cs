using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIView : MonoBehaviour
{
    [SerializeField]
    CanvasGroup canvasGroup;
    
    [SerializeField]
    string UIViewName;

    VisibleState state;

    public virtual void Start()
    {
        if (UIViewName == "")
            Debug.LogError("UIView이름을 입력하지 않았습니다.");

        UINavigation.uiNav.PushUIViewDic(UIViewName, this);
    }
    public VisibleState State { get; }        
    
    public virtual void Show() { StartCoroutine(FadeIn()); }    
    public virtual void Hide() { StartCoroutine(FadeOut()); }
    public enum VisibleState
    {
        Appearing,
        Appeared,
        Disappearing,
        Disappeared,
    }
    IEnumerator FadeIn()
    {
        UINavigation.DontTouchScreenOn(this);
        state = VisibleState.Appearing;
        while(canvasGroup.alpha!= 1)
        {
            canvasGroup.alpha += Time.deltaTime;
            yield return null;
        }
        UINavigation.DontTouchScreenOff();
        state = VisibleState.Appeared;
    }
    IEnumerator FadeOut()
    {
        UINavigation.DontTouchScreenOn(this);
        state = VisibleState.Disappeared;
        while (canvasGroup.alpha != 0)
        {
            canvasGroup.alpha -= Time.deltaTime;
            yield return null;
        }        
        UINavigation.DontTouchScreenOff();
        state = VisibleState.Disappeared;
    }
}
