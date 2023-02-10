using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using TMPro;

public class LoginUI : MonoBehaviour
{
    [SerializeField]
    TMP_InputField input_Id;
    [SerializeField]
    TMP_InputField input_Pw;
    [SerializeField]
    GameObject errormessageObj;
    [SerializeField]
    TextMeshProUGUI errorMessage;

    public void OnClikLoginButton()
    {
        string id = input_Id.text;
        string password = input_Pw.text;

        DataManager.datamanager.UserInfoPost(id, password, ErrorMessage,PostDataType.Login);

        input_Pw.text = "";
    }   
    
    bool CheckInput(string _input)
    {
        string inputCheck = Regex.Replace(_input, @"[^a-zA-Z0-9°¡-ÆR\.*,]", "", RegexOptions.Singleline);
        if (_input.Equals(inputCheck) == false)
            return false;
        else
            return true;
        
    }
    void ErrorMessage(PostData _data)
    {
        if (_data.Result != "OK")
        {
            errormessageObj.gameObject.SetActive(true);
            switch(_data.Msg)
            {
                case "NoneID":
                    errorMessage.text = "¾ÆÀÌµð°¡ ¾ø½À´Ï´Ù.";
                    break;
                case "WrongPassword":
                    errorMessage.text = "ºñ¹Ð¹øÈ£°¡ Æ²·È½À´Ï´Ù.";
                    break;
                default:
                    Debug.LogError("Wrong GoogleMsg");
                    break;
            }
        }
    }
  
  
}
