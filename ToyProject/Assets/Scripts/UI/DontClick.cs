using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontClick : MonoBehaviour
{
    [SerializeField]
    ErrorMessage errorMessage;
    private void OnEnable()
    {
        if (errorMessage.gameObject.activeSelf == true)
            errorMessage.gameObject.SetActive(false);
    }
    public void SetErrorMessage(string _msg)
    {
        errorMessage.gameObject.SetActive(true);
        errorMessage.gameObject.transform.localPosition = Vector3.zero;
        errorMessage.SetErrorMessage(_msg);            
    }
    public void OnClickCloseButton()
    {
        gameObject.SetActive(false);
    }
}
