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
        UIManager.Instance.ALobbyPlayerSetting += SetNickName;
        AuthManager.Instance.LobbyMainSetting();
    }
    private void OnDisable()
    {
        UIManager.Instance.ALobbyPlayerSetting -= SetNickName;
    }
    
    void SetNickName(UserInfo _nickName) { idText.text = _nickName.NickName; }
    
}