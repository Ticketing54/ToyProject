using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
public class LoadingUI : MonoBehaviour
{
    /// <summary>
    /// �ε� �̹��� �����ҰŸ� �ٲ� �� 
    /// </summary>
    [SerializeField]
    Image backGround;


    // �ε� �̹����� ���α׷�����
    [SerializeField]
    Image simpleLoadingImage;
    [SerializeField]
    GameObject progress;

    // �ش� ��ġ
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
    #endregion

    Coroutine updateCo;
    public void OpenLoadingUI(bool _isProgress = false)
    {
        gameObject.SetActive(true);
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
        Clear();
        gameObject.SetActive(false);
    }
    private void Clear()
    {
        simpleLoadingImage.transform.SetParent(outPosition);
        progress.transform.SetParent(outPosition);
        progressText.text = "";
        progressBar.fillAmount = 0;
    }
    IEnumerator CoRotateLoadingImage()
    {
        while(true)
        {
            yield return null;
            simpleLoadingImage.transform.Rotate(Vector3.forward * 2f);
        }
    }
    public void UpdateProgress (float _current, float _target, Action continueWith=null)
    {
        if(updateCo != null)
        {
            StopCoroutine(updateCo);
        }   
        updateCo = StartCoroutine(CoUpdateProgress(_current,_target)); 
    }
    public IEnumerator CoUpdateProgress(float _current, float _target, Action continueWith = null)
    {
        
        float percent = _current / _target;
        float curPercent = progressBar.fillAmount;
        while (progressBar.fillAmount < percent)
        {
            yield return null;
            progressBar.fillAmount = Mathf.Lerp(curPercent, percent, 0.1f);
            progressText.text = (progressBar.fillAmount * 100).ToString("F2") +" %";
        }
        if(continueWith != null)
        {
            continueWith();
        }
    }
   
}
