using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NavSign : UINavigation
{
    [SerializeField]
    UIView patch;
    [SerializeField]
    UIView login;
    [SerializeField]
    UIView regist;
    
    private void Start()
    {
        UIManager.uiManager.OpenLoginUI += OpenLogin;
        UIManager.uiManager.OpenPatchUI += OpenPatch;
    }
    void StartLoginUI(bool _patch)
    {
        if(_patch)
        {
            patch.gameObject.SetActive(true);
        }
        else
        {
            login.gameObject.SetActive(true);
        }
    }    
    public void OnClickRegistButton() { Push(regist); }    
    public void OnClickPopButton() { Pop(); }
    
    private void OpenLogin()
    {
        if (patch.gameObject.activeSelf == true)
        {
            patch.gameObject.SetActive(false);
        }            
        current = login;
        login.gameObject.SetActive(true);
        login.Show();
    }
    private void OpenPatch(long _size)
    {
        current = patch;
        patch.gameObject.SetActive(true);
        PatchUI patchUI = patch as PatchUI;
        patchUI.SetPatchUI(_size);
        patch.Show();
    }
}
