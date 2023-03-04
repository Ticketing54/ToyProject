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
    
    public override void Awake()
    {
        base.Awake();            
    }
    private void OnEnable()
    {
        GameManager.instance.CheckPatch(OpenLoginStart,OpenPatchStart);
    }
    
    void OpenLoginStart()
    {
        if(current != null)
        {
            current.gameObject.SetActive(false);            
        }
        login.gameObject.SetActive(true);
        login.Show();
        current = login;
    }
    /// <summary>
    /// Download Size
    /// </summary>
    /// <param name="DownloadSize"></param>
    void OpenPatchStart(long _size)
    {
        patch.gameObject.SetActive(true);
        PatchUI temp = (PatchUI)patch;
        temp.SetPatchUI(_size);
        patch.Show();
        current = patch;
    }

    public void OnClickRegistButton() { Push(regist); }    
    public void OnClickPopButton() { Pop(); }
}
