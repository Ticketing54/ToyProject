using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Networking;

public class DataManager : MonoBehaviour
{
    public static DataManager datamanager;

    string URL = "https://script.google.com/macros/s/AKfycbzi1ju790oFWwEvBlNgKpeE3nm73HCIChnd8k_D9AixmmGH7_DWsNY3JblPAOEeKGcW/exec";

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
    
    public void UserInfoPost(string _id,string _pw,Action<PostData> _ui,PostDataType _postDatatype)
    {
        WWWForm form = new WWWForm();

        switch(_postDatatype)
        {
            case PostDataType.Login:
                {
                    form.AddField("order", "login");
                    break;
                }
            case PostDataType.Regist:
                {
                    form.AddField("order", "register");
                    break;
                }
            default:
                break;
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
