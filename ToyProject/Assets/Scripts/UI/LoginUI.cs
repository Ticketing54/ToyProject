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
    
    public void OnClikLoginButton()
    {
        string id = input_Id.text;
        string password = input_Pw.text;

        DataManager.datamanager.Post(id, password, PushMessage, false);

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
    void PushMessage(PostData _data)
    {
        Debug.Log(_data.Order);
        Debug.Log(_data.Msg);
        Debug.Log(_data.Result);        
    }
    
}
