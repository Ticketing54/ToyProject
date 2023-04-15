using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
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
