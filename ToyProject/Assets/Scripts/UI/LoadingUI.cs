using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingUI : MonoBehaviour
{   
    [SerializeField]
    Image backGround;
    [SerializeField]
    Image loadingImage;
    [SerializeField]
    Image loadingProgress;

    private float targetProgress;
    private float currentProgress;
    public void StartLoading()
    {
        gameObject.SetActive(true);
        loadingImage.gameObject.SetActive(true);
        StartCoroutine(CoRotateLoadingImage());
    }
    public void StartLoadingProgress()
    {
        gameObject.SetActive(true);
        loadingProgress.gameObject.SetActive(true);
    }
    private void Clear()
    {
        loadingImage.gameObject.SetActive(false);
        loadingProgress.gameObject.SetActive(false);
        targetProgress = 0;
        currentProgress = 0;
    }

    IEnumerator CoRotateLoadingImage()
    {
        while(true)
        {
            yield return null;
            loadingImage.transform.Rotate(Vector3.forward * 2f);
        }
    }

   
}
