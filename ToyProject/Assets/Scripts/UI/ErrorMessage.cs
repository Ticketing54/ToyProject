using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ErrorMessage : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI errorMessage;
    [SerializeField]
    private void OnEnable()
    {
        errorMessage.text = "";        
    }

    /// <summary>
    /// DontClick 을 활성화 시킨 후에 쓸것! 자식오브젝트에 붙어있음
    /// </summary>
    /// <param name="Message"></param>
    public void SetErrorMessage(string _msg)
    {
        errorMessage.gameObject.SetActive(true);
        errorMessage.text = _msg;
    }
}
