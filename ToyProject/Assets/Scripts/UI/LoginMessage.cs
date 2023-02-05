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
        text.text = "ID가 존재하지 않습니다.";
    }    
    public void LoginFail_PW()
    {
        text.text = "패스워드가 틀렸습니다.";
    }
    public void RegistSuccess()
    {
        text.text = "ID 생성 완료!";
    }
    public void RegistFail()
    {
        text.text = "ID를 사용할 수 없습니다.";
    }
    
}
