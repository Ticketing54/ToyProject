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
    public void SetErrorMessage(string _msg)
    {
        errorMessage.gameObject.SetActive(true);
        errorMessage.text = _msg;
    }
}
