using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInvitationRequestList : MonoBehaviour
{
    [SerializeField]
    UIInvitationSlot sampleSlot;
    [SerializeField]
    GameObject contents;
    Stack<UIInvitationSlot> activeSlot;

    private void Awake()
    {
        activeSlot = new();
    }
    private void OnEnable()
    {
        Clear();
        UIManager.uiManager.AOpenInvitationMessage += Add;
    }
    private void OnDisable()
    {
        UIManager.uiManager.AOpenInvitationMessage -= Add;
    }

    void Add(UserInfo _userinfo,string _roomName)
    {
        UIInvitationSlot slot = Instantiate(sampleSlot);
        slot.gameObject.SetActive(true);
        slot.transform.SetParent(contents.transform);
        slot.transform.localScale = new Vector3(1f, 1f, 1f);
        slot.RoomName = _roomName;
        slot.SetProfile(_userinfo);
        activeSlot.Push(slot);
    }
    void Clear()
    {
        while(activeSlot.Count != 0)
        {
            UIInvitationSlot slot = activeSlot.Pop();
            if(slot != null)
            {
                Destroy(slot);
            }
        }

    }


}
