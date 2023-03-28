using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UVLobbyMain : UIView
{
    [SerializeField]
    TextMeshProUGUI idText;
    private void OnEnable()
    {   
        AuthManager.Instance.AUserSetting += SetNickName;
        AuthManager.Instance.LobbyMainSetting();
    }
    private void OnDisable()
    {   
        AuthManager.Instance.AUserSetting -= SetNickName;
    }
    
    void SetNickName(UserInfo _nickName) { idText.text = _nickName.NickName; }
}
