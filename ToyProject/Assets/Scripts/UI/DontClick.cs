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
        if (errorMessage.gameObject.activeSelf == true)
            errorMessage.gameObject.SetActive(false);                

    }    
    public void SetErrorMessage(string _data)
    {
        errorMessage.gameObject.SetActive(true);
        errorMessage.gameObject.transform.localPosition = Vector3.zero;
        errorMessage.SetErrorMessage(_data);            
    }    
}
