using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIView : MonoBehaviour
{
    [SerializeField]
    CanvasGroup canvasGroup;
    

    public virtual IEnumerator Show() { yield return StartCoroutine(FadeIn()); }    
    public virtual IEnumerator Hide() { yield return StartCoroutine(FadeOut()); }
    //public enum VisibleState
    //{
    //    Appearing,
    //    Appeared,
    //    Disappearing,
    //    Disappeared,
    //}
    // 사용 될때 기다리기
    IEnumerator FadeIn()
    {   
        //state = VisibleState.Appearing;
        while(canvasGroup.alpha!= 1)
        {
            canvasGroup.alpha += Time.deltaTime;
            yield return null;
        }        
        //state = VisibleState.Appeared;
    }
    IEnumerator FadeOut()
    {   
        //state = VisibleState.Disappeared;
        while (canvasGroup.alpha != 0)
        {
            canvasGroup.alpha -= Time.deltaTime;
            yield return null;
        }        
        //tate = VisibleState.Disappeared;
    }
}
