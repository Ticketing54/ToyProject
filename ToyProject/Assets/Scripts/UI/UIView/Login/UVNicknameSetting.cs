using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UVNicknameSetting : UIView
{
    [SerializeField]
    TMP_InputField nickNameInput;
    
    
    private void Start()
    {
        nickNameInput.onValueChanged.AddListener(OnVlaueChagned);
    }
    void OnVlaueChagned(string _value)
    {
        string nickName = string.Empty;
        foreach(char c in _value)
        {
            if(char.IsLetterOrDigit(c))
            {
                nickName += c;
            }
        }
        nickNameInput.text = nickName;
    }
    public void OnClickNickNameSettingButton()
    {
        AuthManager.Instance.SetNickName(nickNameInput.text);
    }
    enum NICKERROR
    {
        SCPACING,
        OVERLENGTH,
    }

}
