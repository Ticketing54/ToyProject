using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRequest : MonoBehaviour
{

    [SerializeField]
    UIRequestSlot sampleSlot;
    [SerializeField]
    GameObject Contents;
    HashSet<UIRequestSlot> activeSlots;
    private void Awake()
    {
        activeSlots = new HashSet<UIRequestSlot>();
    }
    private void OnEnable()
    {
        Clear();
        AuthManager.Instance.ACheckFriendRequests += Add;
        AuthManager.Instance.CheckFriendRequests();
    }
    private void OnDisable()
    {
        AuthManager.Instance.ACheckFriendRequests -= Add;
    }
    
    void Add(string _userId,string _nickName)
    {
        UIRequestSlot slot = Instantiate<UIRequestSlot>(sampleSlot);
        slot.gameObject.SetActive(true);
        slot.transform.SetParent(Contents.transform);
        slot.transform.localScale = new Vector3(1, 1, 1);
        slot.SetProfile(_nickName, _userId);
        activeSlots.Add(slot);
    }
    void Clear()
    {
        foreach(UIRequestSlot one in activeSlots)
        {
            if(one != null)
            {
                Destroy(one.gameObject);
            }
            
        }
        activeSlots.Clear();
    }
}
