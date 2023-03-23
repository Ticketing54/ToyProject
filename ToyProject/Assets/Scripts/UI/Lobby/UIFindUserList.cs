using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UIFindUserList : MonoBehaviour
{
    [SerializeField]
    TMP_InputField nickNameintput;


    private void OnEnable()
    {
        Clear();
        AuthManager.Instance.AUpdateFindUser += Add;
    }
    private void OnDisable()
    {
        AuthManager.Instance.AUpdateFindUser -= Add;
    }

    public void OnClickFindButton()
    {
        if (nickNameintput.text == "")
            return;

        string nicName = nickNameintput.text;
        nickNameintput.text = "";
    }
    
    void Add(UserInfo _userInfo,bool _alreadySend)
    {
        
    }
    void Clear()
    {

    }
}
