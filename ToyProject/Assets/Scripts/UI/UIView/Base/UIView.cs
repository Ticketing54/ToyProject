using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIView : MonoBehaviour
{
    [SerializeField]
    protected CanvasGroup canvasGroup;
    public VisibleState State { get => state; }
    protected VisibleState state;
    public virtual void Show() { StartCoroutine(FadeIn()); }
    public virtual void Hide() { StartCoroutine(FadeOut()); }
    public enum VisibleState
    {
        Appearing,
        Appeared,
        Disappearing,
        Disappeared,
    }
    
    protected virtual IEnumerator FadeIn()
    {   
        state = VisibleState.Appearing;
        while(canvasGroup.alpha!= 1)
        {
            yield return null;
            canvasGroup.alpha += Time.deltaTime*0.7f;            
        }        
        state = VisibleState.Appeared;
    }
    protected virtual IEnumerator FadeOut()
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
