using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
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
    #endregion


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
        simplePosition.transform.SetParent(outPosition);
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

   
}
