using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class LoginManager : MonoBehaviour
{
    const string URL = "https://docs.google.com/spreadsheets/d/1r84Xh15BPtjMQk5m38sNB5co3IJSqDIKUCTtDHB8Nkc/export?format=tsv&range=A2:B";
    
    public void DoubleCheck()
    {
        //check Id

        WWWForm form = new WWWForm();
        form.AddField("order", "check");
        //form.AddField("id",)
    }
    public void Login()
    {

    }
   
    public void Regist()
    {
        
    }
}
