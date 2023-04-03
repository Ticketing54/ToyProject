using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRequest : MonoBehaviour
{

    [SerializeField]
    UIRequestSlot sampleSlot;
    [SerializeField]
    GameObject Contents;
    Stack<UIRequestSlot> activeSlots;
    private void Awake()
    {
        activeSlots = new Stack<UIRequestSlot>();
    }
    private void OnEnable()
    {
        Clear();
        UIManager.uiManager.ACheckFriendRequests += Add;
        AuthManager.Instance.CheckFriendRequests();
    }
    private void OnDisable()
    {
        UIManager.uiManager.ACheckFriendRequests -= Add;
    }
    
    void Add(UserInfo _userinfo)
    {
        UIRequestSlot slot = Instantiate<UIRequestSlot>(sampleSlot);
        slot.gameObject.SetActive(true);
        slot.transform.SetParent(Contents.transform);
        slot.transform.localScale = new Vector3(1, 1, 1);
        slot.SetProfile(_userinfo);
        activeSlots.Push(slot);
    }
    void Clear()
    {
       while(activeSlots.Count != 0)
        {
            UIRequestSlot popSlot = activeSlots.Pop();
            if(popSlot != null)
            {
                Destroy(popSlot.gameObject);
            }
        }
    }
}
