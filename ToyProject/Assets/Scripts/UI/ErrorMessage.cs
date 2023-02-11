using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ErrorMessage : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI errorMessage;
    
    public void OnClickButton()
    {
        errorMessage.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
    public void SetErrorMessage(string _result)
    {
        errorMessage.gameObject.SetActive(true);
        errorMessage.text = _result;
        if (_result != "OK")
        {
            switch (_result)
            {
                case "NoneID":
                    errorMessage.text = "���̵� �����ϴ�.";
                    break;
                case "WrongPassword":
                    errorMessage.text = "��й�ȣ�� Ʋ�Ƚ��ϴ�.";
                    break;
                default:
                    Debug.LogError("Wrong GoogleMsg");
                    break;
            }
        }
    }

}
