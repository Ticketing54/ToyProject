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
        UIManager.Instance.AOpenRoom += () => { Push(playRoom); };
    }
    public void OnClickCreateRoom() { LobbyManager.Instance.CreatRoom(); }
    public void OnClickTutorialButton()
    {
        // ���� ����
    }
    

}