using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Register : MonoBehaviour
{
    public void OnColickRegisterButton()
    {
        WWWForm form = new WWWForm();
        form.AddField("order", "register");
        form.AddField("id", "TestId");
        form.AddField("password", "1111122");

        StartCoroutine(Post(form));
    }
    IEnumerator Post(WWWForm form)
    {
        string url = "https://script.google.com/macros/s/AKfycbzablwZk-pJVg5cMHlApAqIrK4PnAtOwuOjKDTmKpQc_jq8YfIQ0yDcSpTU55dkoDHh/exec";

        yield return null;
    }
}
