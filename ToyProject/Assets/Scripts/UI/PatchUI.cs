using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PatchUI : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI sizeUI;
    [SerializeField]
    GameObject patchBox;
    [SerializeField]
    GameObject downloadMessageUI;
    [SerializeField]
    GameObject downloadProgress;

    [SerializeField]
    Image progressBar;
    [SerializeField]
    TextMeshProUGUI ProgressMessage;

    public void OpenPatchUI()
    {
        gameObject.SetActive(true);
    }
    public void ClosePatchUI()
    {
        Clear();
        gameObject.SetActive(false);
    }
    void Clear()
    {
        if (downloadMessageUI.gameObject.activeSelf == true)
        {
            downloadMessageUI.gameObject.SetActive(false);
        }
        if (downloadProgress.activeSelf == true)
        {
            downloadProgress.SetActive(false);
        }
    }
    public void SetPatchUI(long _size)
    {
        patchBox.gameObject.SetActive(true);
        downloadMessageUI.gameObject.SetActive(true);
        sizeUI.text = ((float)_size / (1024f * 1024f)).ToString("F2") + " MB";
    }

    public void OnClickUpdateButton()
    {
        downloadMessageUI.gameObject.SetActive(false);
        downloadProgress.gameObject.SetActive(true);
        GameManager.Instance.DownloadPatch();
    }
    public void OnClickCancleButton()
    {
        Application.Quit();
    }
    /// <summary>
    /// �ٿ������, ũ��, ���൵
    /// </summary>
    /// <param name="Current"></param>
    /// <param name="Size"></param>
    /// <param name="FillAmount"></param>
    public void UpdatePatch(long _cur, long _size, float _percent)
    {
        string cur = ((float)_cur / (1024f * 1024f)).ToString("F2") +" MB";
        string size = ((float)_size / (1024f * 1024f)).ToString("F2")+" MB";
        ProgressMessage.text = cur + " / " + size + "  [" + _percent * 100 +" % "+"]";
        progressBar.fillAmount = _percent;
    }

}
