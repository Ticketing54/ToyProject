using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using TMPro;

public class UVLogin :UIView
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
            UIManager.Instance.OnErrorMessage("E-Mail과 Password를 입력해 주세요.");
            return;
        }
        AuthManager.Instance.Login(id, password);
        input_Pw.text = "";
    }
   
    bool CheckInput(string _input)
    {
        string inputCheck = Regex.Replace(_input, @"[^a-zA-Z0-9가-힣\.*,]", "", RegexOptions.Singleline);
        if (_input.Equals(inputCheck) == false)
            return false;
        else
            return true;        
    }
    
  
}
