using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInvitationSlot : UIUserSlot
{
    public string RoomName { get; set; }
    public override void SetProfile(UserInfo _userinfo)
    {
        userId = _userinfo.UID;
        nickName.text = _userinfo.NickName +" ���κ��� �ʴ�޼����Դϴ�.";
        StartCoroutine(CoCountDestroy());
    }
    public void OnClickAcceptButton()
    {
        LobbyManager.Instance.JoinRoom(RoomName);
        Destroy(this.gameObject);
    }
    public void OnClickRefuseButton()
    {   
        Destroy(this.gameObject);
    }
    IEnumerator CoCountDestroy()
    {
        yield return new WaitForSeconds(10f);
        Destroy(this.gameObject);
    }
}
