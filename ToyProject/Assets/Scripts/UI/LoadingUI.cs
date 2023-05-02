using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
public class LoadingUI : MonoBehaviour
{
    /// <summary>
    /// 로딩 이미지 설정할거면 바꿀 것 
    /// </summary>
    [SerializeField]
    Image backGround;


    // 로딩 이미지와 프로그래스바
    [SerializeField]
    Image simpleLoadingImage;
    [SerializeField]
    GameObject progress;
    [SerializeField]
    Image waitOtherPlayerMessage;

    // 해당 위치
    [SerializeField]
    Transform outPosition;
    [SerializeField]
    Transform simplePosition;
    [SerializeField]
    Transform progressPosition;


    #region Progress
    [SerializeField]
    Image progressBar;
    [SerializeField]
    TextMeshProUGUI progressText;

    float targetStep;
    float currentStep;
    #endregion
    public void OpenLoadingUI(bool _isProgress = false)
    {
        gameObject.SetActive(true);
        Clear();
        if(_isProgress)
        {
            progress.transform.SetParent(progressPosition);
        }
        else
        {
            simpleLoadingImage.transform.SetParent(simplePosition);
            StartCoroutine(CoRotateLoadingImage());
        }
    }

    public void CloseLoadingUI()
    {   
        gameObject.SetActive(false);
    }
    private void Clear()
    {
        StopAllCoroutines();
        simpleLoadingImage.transform.SetParent(outPosition);
        progress.transform.SetParent(outPosition);
        waitOtherPlayerMessage.transform.SetParent(outPosition);
        progressText.text = "";
        progressBar.fillAmount = 0;
        targetStep = 0;
        currentStep = 0;
    }
    IEnumerator CoRotateLoadingImage()
    {
        while(true)
        {
            yield return null;
            simpleLoadingImage.transform.Rotate(Vector3.forward * 2f);
        }
    }
    public void ProgressSetting (float _targetProgress)
    {
        targetStep = _targetProgress;
        currentStep = 0;
        StartCoroutine(CoUpdateProgress()); ; 
    }
    public float CurrentStep { get => currentStep; set => currentStep = value; }
    
    public IEnumerator CoUpdateProgress()
    {
        while (progressBar.fillAmount < 1)
        {
            yield return null;
            float percent = currentStep / targetStep;
            if(progressBar.fillAmount < percent)
            {
                progressBar.fillAmount += Time.deltaTime*0.5f;
                progressText.text = (progressBar.fillAmount * 100).ToString("F2") + " %";
            }
        }
        simpleLoadingImage.transform.SetParent(outPosition);
        progress.transform.SetParent(outPosition);
        progressText.text = "";
        progressBar.fillAmount = 0;
        targetStep = 0;
        currentStep = 0;
        waitOtherPlayerMessage.transform.SetParent(progressPosition);
    }
   
}
