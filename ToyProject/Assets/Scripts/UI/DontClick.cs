using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DontClick : MonoBehaviour
{
    [SerializeField]
    ErrorMessage errorMessage;
    [SerializeField]
    Image clickRange;
    private void OnEnable()
    {
        if (errorMessage.gameObject.activeSelf == true)
            errorMessage.gameObject.SetActive(false);                
    }
    public void SetRange(Vector2 _range) { clickRange.rectTransform.sizeDelta = _range; }
    public void SetErrorMessage(string _data)
    {
        errorMessage.gameObject.SetActive(true);
        errorMessage.gameObject.transform.localPosition = Vector3.zero;
        errorMessage.SetErrorMessage(_data);            
    }
    public void OnClickCloseButton()
    {
        gameObject.SetActive(false);
    }
}
