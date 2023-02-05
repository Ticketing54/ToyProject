using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class LoginManager : MonoBehaviour
{
    string URL = "https://script.google.com/macros/s/AKfycbwqlDjWePQi1heqNZiyX3OJZMoqka_JhsTUrY7zzG0GWQp4U_wGduTK1D2nFZn6o4PP/exec";

    [SerializeField]
    private InputField input_ID;
    [SerializeField]
    private InputField input_Pw;

    private void Start()
    {
        WWWForm form = new WWWForm();
        form.AddField("order", "login");
        form.AddField("id", "ȫ�浿");
        form.AddField("password", "123123");
        StartCoroutine(CoPos(form));
    }
    public  void OnClickLogin()
    {
        WWWForm form = new WWWForm();
        form.AddField("order", "login");
        form.AddField("id", input_ID.text);
        form.AddField("password", input_Pw.text);        
    }
    
    IEnumerator CoPos(WWWForm form)
    {   
        using (UnityWebRequest www = UnityWebRequest.Post(URL,form))
        {
            yield return www.SendWebRequest();

            if (www.isDone)
            {   
                LoginMessage json = JsonUtility.FromJson<LoginMessage>(www.downloadHandler.text);                
                switch(json.TryLogin())
                {
                    case LoginMessage.LogInMsg.LogInSucess:
                        break;
                    case LoginMessage.LogInMsg.NonExistent:
                        break;
                    case LoginMessage.LogInMsg.WrongPassword:
                        break;
                    default:
                        // Error Message
                        break;
                }
            }
            else
            {
                // Error Message
            }
        }
    }
        
    public void Regist()
    {

    }   
}
