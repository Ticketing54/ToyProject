using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingUI : MonoBehaviour
{
    [SerializeField]
    CanvasGroup loadingAlpha;
    [SerializeField]
    Image backGround;
    [SerializeField]
    Image loadingImage;

    [SerializeField]
    Coroutine uiEffect;

    private void OnEnable()
    {
        if(uiEffect != null)
        {
            StopCoroutine(uiEffect);
            uiEffect = null;
        }
    }
    public void OpenLoading()
    {
        StartCoroutine(CoFadeOut());
        uiEffect = StartCoroutine(CoLoading());
    }
    // LoadingUI를 종료할떄  꼭 사용할 것
    public void CoseLoading() { StartCoroutine(CoExitLoadingUI()); }
    
    IEnumerator CoExitLoadingUI()
    {
        yield return CoFadeIn();
        StopCoroutine(uiEffect);
        gameObject.SetActive(false);
    }
    IEnumerator CoFadeIn()
    {
        while(loadingAlpha.alpha > 0)
        {
            yield return null;
            loadingAlpha.alpha -= Time.deltaTime;
        }
    }
    IEnumerator CoFadeOut()
    {
        while (loadingAlpha.alpha < 1)
        {
            yield return null;
            loadingAlpha.alpha += Time.deltaTime;
        }
    }
    IEnumerator CoLoading()
    {
        while(true)
        {
            yield return null;
            loadingImage.transform.Rotate(Vector3.forward * 2f);
        }
    }

}
