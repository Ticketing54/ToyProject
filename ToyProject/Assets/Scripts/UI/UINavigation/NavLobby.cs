using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavLobby : UINavigation
{
    [SerializeField]
    UIView playRoom;
    [SerializeField]
    UIView nickNameSetting;
    public override void Awake()
    {
        base.Awake();
        AuthManager.Instance.AOpenRoom += () => { Push(playRoom); };
    }
    public override void RootShow()
    {
        base.RootShow();
    }
    public void OnClickCreateRoom() { LobbyManager.Instance.CreatRoom(); }
    public void OnClickTutorialButton()
    {
        // 제작 예정
    }
    

}
