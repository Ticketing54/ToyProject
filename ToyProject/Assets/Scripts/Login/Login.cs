using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class Login : MonoBehaviour
{
    public void OnClickLoginButton()
    {
        WWWForm form = new WWWForm();
        form.AddField("order", "login");
        form.AddField("id", "TestId");
        form.AddField("password", "1111122");

        StartCoroutine(Post(form));
    }

    IEnumerator Post(WWWForm form)
    {
        string url = "https://script.google.com/macros/s/AKfycbzablwZk-pJVg5cMHlApAqIrK4PnAtOwuOjKDTmKpQc_jq8YfIQ0yDcSpTU55dkoDHh/exec";
        using (UnityWebRequest www = UnityWebRequest.Post(url,form))
        {
            yield return www.SendWebRequest();
            if (www.isDone)
                print(www.downloadHandler.text);
            else
                print("Erorr");
        }
    }
}
