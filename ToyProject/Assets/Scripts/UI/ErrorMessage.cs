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
    Button registButton;
    [SerializeField]
    Button exitButton;
   
    private void OnEnable()
    {
        if (registButton.gameObject.activeSelf == true)
            registButton.gameObject.SetActive(false);
        if (registButton.gameObject.activeSelf == true)
            exitButton.gameObject.SetActive(false);
    }

    public void SetErrorMessage(PostData _postData)
    {
        errorMessage.gameObject.SetActive(true);


        switch(_postData.Order)
        {
            case "login":
                {
                    exitButton.gameObject.SetActive(true);
                    switch (_postData.Msg)
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
                break;
            case "register":
                {
                    if(_postData.Result == "OK")
                    {
                        registButton.gameObject.SetActive(true);
                        errorMessage.text = "������ �Ϸ�Ǿ����ϴ�.";
                    }
                    else
                    {
                        exitButton.gameObject.SetActive(true);
                        errorMessage.text = "���̵� �ߺ��˴ϴ�.";
                    }                    
                    break;
                }
        }        
    }

}
