using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using TMPro;

public class LoginUI :UIView
{   
    [SerializeField]
    TMP_InputField input_Id;
    [SerializeField]
    TMP_InputField input_Pw;   
    

    public void OnClikLoginButton()
    {
        string id = input_Id.text;
        string password = input_Pw.text;
        if(id == ""|| password == "")
        {
            
        }
        DataManager.datamanager.UserInfoPost(id, password, ErrorMessage,PostDataType.Login);

        input_Pw.text = "";
    }
    void ErrorMessage(PostData _data)
    {

    }
   
    bool CheckInput(string _input)
    {
        string inputCheck = Regex.Replace(_input, @"[^a-zA-Z0-9°¡-ÆR\.*,]", "", RegexOptions.Singleline);
        if (_input.Equals(inputCheck) == false)
            return false;
        else
            return true;
        
    }
    
  
}
