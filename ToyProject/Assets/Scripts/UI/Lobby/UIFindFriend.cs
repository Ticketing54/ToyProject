using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UIFindFriend : MonoBehaviour
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
        AuthManager.Instance.UpdateFindUser(nicName);
    }
    
    void Add(UserInfo _userInfo)
    {

    }
    void Clear()
    {

    }
}
