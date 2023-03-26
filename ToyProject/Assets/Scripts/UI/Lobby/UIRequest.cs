using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRequest : MonoBehaviour
{

    [SerializeField]
    UIRequestSlot sampleSlot;
    [SerializeField]
    GameObject Contents;
    List<UIRequestSlot> activeSlots;
    private void Awake()
    {
        activeSlots = new List<UIRequestSlot>();
    }
    private void OnEnable()
    {
        Clear();
        CheckFriendRequest();
    }

    void CheckFriendRequest()
    {
        AuthManager.Instance.CheckFriendRequests(Add);
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
            Destroy(one);
        }
        activeSlots.Clear();
    }
}
