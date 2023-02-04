using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

[System.Serializable]
class LoginMessage
{
    public string order { get; set; }
    public string result { get; set; }
    public string msg { get; set; }    
}
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
    class UserData
    {
        public string id { get; }
        public string password { get; }
    }
    IEnumerator CoPos(WWWForm form)
    {   
        using (UnityWebRequest www = UnityWebRequest.Post(URL,form))
        {
            yield return www.SendWebRequest();

            if (www.isDone)
            {   
                LoginMessage json = JsonUtility.FromJson<LoginMessage>(www.downloadHandler.text);
                Debug.Log(json.order);
                Debug.Log(json.result);
            }
            else
            {
                // errorMessage
            }
        }
    }
        
    public void Regist()
    {

    }   
}
