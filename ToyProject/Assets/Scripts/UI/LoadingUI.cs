using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingUI : UIView
{   
    [SerializeField]
    Image backGround;
    [SerializeField]
    Image loadingImage;
    

    private void Update()
    {
        loadingImage.transform.Rotate(Vector3.forward * 2f);
    }
    public override void Show()
    {   
        // background sprite 변경 시  load할것
        base.Show();        
    }

    protected override IEnumerator FadeIn()
    {
        state = VisibleState.Appearing;
        while (canvasGroup.alpha != 1)
        {
            yield return null;
            canvasGroup.alpha += Time.deltaTime;
        }
        state = VisibleState.Appeared;
    }
    protected override IEnumerator FadeOut()
    {
        state = VisibleState.Disappearing;
        while (canvasGroup.alpha != 0)
        {
            yield return null;
            canvasGroup.alpha -= Time.deltaTime * 0.7f;
        }
        state = VisibleState.Disappeared;
        this.gameObject.SetActive(false);
    }
}
