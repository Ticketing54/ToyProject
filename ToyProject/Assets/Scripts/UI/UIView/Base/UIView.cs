using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIView : MonoBehaviour
{
    [SerializeField]
    CanvasGroup canvasGroup;
    public VisibleState State { get => state; }
    VisibleState state;
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
        state = VisibleState.Appearing;
        while(canvasGroup.alpha!= 1)
        {
            yield return null;
            canvasGroup.alpha += Time.deltaTime*0.7f;            
        }        
        state = VisibleState.Appeared;
    }
    IEnumerator FadeOut()
    {   
        state = VisibleState.Disappearing;
        while (canvasGroup.alpha != 0)
        {   
            yield return null;
            canvasGroup.alpha -= Time.deltaTime*0.7f;
        }        
        state = VisibleState.Disappeared;
    }
}
