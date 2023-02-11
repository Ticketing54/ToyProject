using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
public class LoginRegist : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI errorMsg_ID;
    [SerializeField]
    TextMeshProUGUI errorMsg_PW;    

    [SerializeField]
    TMP_InputField input_ID;
    [SerializeField]
    TMP_InputField input_PW;
    [SerializeField]
    TMP_InputField input_PWC;

    [SerializeField]
    Image completeMessage;

    private void OnEnable()
    {
        Reset();
    }
    public void OnClickRegistButton()
    {
        ResetErrorMsg();
        if (input_ID.text == "" || input_PW.text == "")
        {
            errorMsg_ID.gameObject.SetActive(true);
            errorMsg_PW.gameObject.SetActive(true);
            errorMsg_PW.text = "��й�ȣ�� Ȯ���� �ּ���.";
            errorMsg_ID.text = "ID�� Ȯ���� �ּ���.";
            return;
        }            
        if(input_PW.text != input_PWC.text)
        {
            errorMsg_PW.gameObject.SetActive(true);
            errorMsg_PW.text = "��й�ȣ�� Ȯ���� �ּ���.";
            input_PWC.text = "";           
            return;
        }
        DataManager.datamanager.UserInfoPost(input_ID.text, input_PW.text, CompleteMessage, PostDataType.Regist);
    }
    public void OnClickCompleteMessage()
    {
        completeMessage.gameObject.SetActive(false);
        ResetErrorMsg();        
        gameObject.SetActive(false);
    }
    public void OnClickCloseButton()
    {
        ResetErrorMsg();        
        gameObject.SetActive(false);
    }
    void CompleteMessage(PostData _postData)
    {
        if (_postData.Result == "OK")
        {   
            completeMessage.gameObject.SetActive(true);            
        }
        else
        {
            errorMsg_ID.gameObject.SetActive(true);
            errorMsg_ID.text = "�ߺ��Ǵ� ���̵��Դϴ�.";
        }
    }

    void ResetErrorMsg()
    {
        if(errorMsg_ID.gameObject.activeSelf == true)
            errorMsg_ID.gameObject.SetActive(false);
        if (errorMsg_PW.gameObject.activeSelf == true)
            errorMsg_PW.gameObject.SetActive(false);        
    }
    private void Reset()
    {
        ResetErrorMsg();
        input_ID.text = "";
        input_PW.text = "";
        input_PWC.text = "";
    }
}