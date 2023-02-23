using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DontClick : MonoBehaviour
{
    [SerializeField]
    ErrorMessage errorMessage;
    private void OnEnable()
    {
        if(errorMessage.gameObject.activeSelf == true)
        {
            errorMessage.gameObject.SetActive(false);
        }
    }
    /// <summary>
    /// 오류 메세지 출력 
    /// </summary>
    /// <param name="Message"></param>
    public void SetErrorMessage(string _msg)
    {
        errorMessage.gameObject.SetActive(true);
        errorMessage.gameObject.transform.localPosition = Vector3.zero;
        errorMessage.SetErrorMessage(_msg);            
    }    
}
