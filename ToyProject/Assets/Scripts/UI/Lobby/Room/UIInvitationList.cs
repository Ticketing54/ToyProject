using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInvitationList : MonoBehaviour
{
    Stack<UIInvitationFriendSlot> activeSlots;
    [SerializeField]
    UIInvitationFriendSlot sampleSlot;
    [SerializeField]
    GameObject contentObj;


    private void Awake()
    {
        activeSlots = new Stack<UIInvitationFriendSlot>();
    }



    void Add(UserInfo _friendinfo)
    {
        UIInvitationFriendSlot newSlot = Instantiate(sampleSlot);
        newSlot.gameObject.SetActive(true);
        newSlot.transform.SetParent(contentObj.transform);
        newSlot.transform.localScale = new Vector3(1, 1, 1);
        newSlot.SetProfile(_friendinfo);
        activeSlots.Push(newSlot);
    }
    private void OnEnable()
    {
        Clear();
        UIManager.uiManager.AFriendAdd += Add;
        AuthManager.Instance.UpdateFriendList();
    }
    private void OnDisable()
    {
        UIManager.uiManager.AFriendAdd -= Add;
    }
    void Clear()
    {
        while(activeSlots.Count != 0)
        {
            UIInvitationFriendSlot slot = activeSlots.Pop();
            if(slot != null)
            {
                Destroy(slot.gameObject);
            }
        }
    }
}
