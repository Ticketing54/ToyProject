using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DontClick : MonoBehaviour
{
    [SerializeField]
    ErrorMessage errorMessage;
    [SerializeField]
    TextMeshProUGUI text;

    float count = 0;
    float timer = 0;
    private void OnEnable()
    {
        if(errorMessage.gameObject.activeSelf == true)
        {
            errorMessage.gameObject.SetActive(false);
        }
    }
    private void Update()
    {
        timer += Time.deltaTime;
        if(timer >=1)
        {
            timer -= 1;
            if(count==3)
            {
                text.text = "";
                count = 0;
            }
            else
            {
                text.text += " .";
                count++;
            }
            
        }
    }
    /// <summary>
    /// ���� �޼��� ��� 
    /// </summary>
    /// <param name="Message"></param>
    public void SetErrorMessage(string _msg)
    {
        errorMessage.gameObject.SetActive(true);
        errorMessage.gameObject.transform.localPosition = Vector3.zero;
        errorMessage.SetErrorMessage(_msg);            
    }    
}
