using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavLobby : UINavigation
{
    [SerializeField]
    UIView playRoom;
    [SerializeField]
    UIView nickNameSetting;

    public override void CloseNavigation()
    {
        throw new System.NotImplementedException();
    }

    public void OnClickCreateRoom() { LobbyManager.Instance.CreatRoom(); }
    public void OnClickTutorialButton()
    {
        // 제작 예정
    }

    public override void OpenNavigation()
    {
        throw new System.NotImplementedException();
    }

    public override UIView Pop()
    {
        throw new System.NotImplementedException();
    }

    public override void Push(UIView _uiview)
    {
        throw new System.NotImplementedException();
    }

    public override void ReturnToRoot()
    {
        throw new System.NotImplementedException();
    }
}
