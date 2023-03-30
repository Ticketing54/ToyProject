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

    }
    public override void RootShow()
    {
        base.RootShow();
    }
    public void OnClickCreateRoom()
    {
        AuthManager.Instance.CreateRoom(CreateRoom);
    }
    void CreateRoom()
    {
        Push(playRoom);

    }
    public void OnClickTutorialButton()
    {

    }
    

}
