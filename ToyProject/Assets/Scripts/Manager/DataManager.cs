using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Networking;

public class DataManager : MonoBehaviour
{
    public static DataManager datamanager;

    string URL = "https://script.google.com/macros/s/AKfycbwqlDjWePQi1heqNZiyX3OJZMoqka_JhsTUrY7zzG0GWQp4U_wGduTK1D2nFZn6o4PP/exec";

    private void Awake()
    {
        if (datamanager == null)
        {
            datamanager = this;
            DontDestroyOnLoad(this.gameObject);
        }            
        else
        {
            Destroy(this);
        }
                
    }
    
    public void Post(string _id,string _pw,Action<PostData> _ui,bool _isRegist)
    {
        WWWForm form = new WWWForm();
        if(_isRegist)
        {
            form.AddField("order", "register");
        }
        else
        {
            form.AddField("order", "login");
        }
        form.AddField("id",_id);
        form.AddField("password", _pw);
        StartCoroutine(CoPost(form, _ui));
    }
    IEnumerator CoPost(WWWForm form,Action<PostData> _ui)
    {
        
        
        using (UnityWebRequest www = UnityWebRequest.Post(URL,form))
        {
            yield return www.SendWebRequest();

            if (www.isDone)
            {
                Debug.Log(www.downloadHandler.text);
                PostData json = JsonUtility.FromJson<PostData>(www.downloadHandler.text);
                
                _ui(json);
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
