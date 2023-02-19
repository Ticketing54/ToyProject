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
                            errorMessage.text = "아이디가 없습니다.";
                            break;
                        case "WrongPassword":
                            errorMessage.text = "비밀번호가 틀렸습니다.";
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
                        errorMessage.text = "가입이 완료되었습니다.";
                    }
                    else
                    {
                        exitButton.gameObject.SetActive(true);
                        errorMessage.text = "아이디가 중복됩니다.";
                    }                    
                    break;
                }
        }        
    }

}
