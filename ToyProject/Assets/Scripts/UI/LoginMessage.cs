using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class LoginMessage : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI text;
    private void OnDisable()
    {
        text.text = "";
    }
    public void OnClickCloseButton()
    {
        gameObject.SetActive(false);
    }
    public void LoginFail_ID()
    {
        text.text = "ID�� �������� �ʽ��ϴ�.";
    }    
    public void LoginFail_PW()
    {
        text.text = "�н����尡 Ʋ�Ƚ��ϴ�.";
    }
    public void RegistSuccess()
    {
        text.text = "ID ���� �Ϸ�!";
    }
    public void RegistFail()
    {
        text.text = "ID�� ����� �� �����ϴ�.";
    }
    
}
