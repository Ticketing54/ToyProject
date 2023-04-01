using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInviteFriendList : MonoBehaviour
{
    Stack<UIInviteFriendSlot> activeSlots;
    [SerializeField]
    UIInviteFriendSlot sampleSlot;
    [SerializeField]
    GameObject contentObj;


    private void Awake()
    {
        activeSlots = new Stack<UIInviteFriendSlot>();
    }



    void Add(UserInfo _friendinfo)
    {
        UIInviteFriendSlot newSlot = Instantiate(sampleSlot);
        newSlot.gameObject.SetActive(true);
        newSlot.transform.SetParent(contentObj.transform);
        newSlot.transform.localScale = new Vector3(1, 1, 1);
        newSlot.SetProfile(_friendinfo);
        activeSlots.Push(newSlot);
    }
    private void OnEnable()
    {
        Clear();
        AuthManager.Instance.UpdateFriendList();
    }

    void Clear()
    {
        while(activeSlots.Count != 0)
        {
            UIInviteFriendSlot slot = activeSlots.Pop();
            if(slot != null)
            {
                Destroy(slot.gameObject);
            }
        }
    }
}
