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

        
    }
    IEnumerator cotest()
    {
        yield return StartCoroutine(AuthManager.Instance.Init());
        check = true;
    }
}
