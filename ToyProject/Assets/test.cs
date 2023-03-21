using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    bool check = false;
    private void Start()
    {
        StartCoroutine(cotest());
    }
    private void Update()
    {
        if (!check)
            return;

        if(Input.GetKeyDown(KeyCode.A))
        {
            AuthManager.Instance.FindUser("test", (id,test) => { Debug.Log(id + " : " + test.nickname + "\n"); });
        }
    }
    IEnumerator cotest()
    {
        yield return StartCoroutine(AuthManager.Instance.Init());
        check = true;
    }
}
