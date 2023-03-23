using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    bool check = false;
    private void Start()
    {
        
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            AuthManager.Instance.Test();
        }
        
    }
    
}
