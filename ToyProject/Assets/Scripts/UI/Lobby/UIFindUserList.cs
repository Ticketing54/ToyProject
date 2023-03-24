using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UIFindUserList : MonoBehaviour
{
    // 검색할 닉네임
    [SerializeField]
    TMP_InputField nickNameintput;
    // SampleFriend
    [SerializeField]
    UIFindUserSlot sampleSlot;
    // Pool
    [SerializeField]
    GameObject poolParent;
    // Contents
    [SerializeField]
    GameObject contents;

    List<UIFindUserSlot> activeSlots;
    Queue<UIFindUserSlot> slotPool;
    
    private void Awake()
    {
        activeSlots = new List<UIFindUserSlot>();
        slotPool = new Queue<UIFindUserSlot>();

    }
    private void OnEnable()
    {
        Clear();
        AuthManager.Instance.AUpdateFindUser += Add;
    }
    private void OnDisable()
    {
        AuthManager.Instance.AUpdateFindUser -= Add;
    }

    public void OnClickFindButton()
    {   
        if (nickNameintput.text == "")
            return;
        Clear();
        string nicName = nickNameintput.text;
        nickNameintput.text = "";
        AuthManager.Instance.FindUserList(nicName);
    }
    
    void Add(string _userID,string _userNickName)
    {
        UIFindUserSlot newSlot = null; // slotPool.Dequeue();
        if (newSlot == null)
        {
            newSlot = Instantiate<UIFindUserSlot>(sampleSlot);
        }
        newSlot.gameObject.SetActive(true);
        newSlot.transform.SetParent(contents.transform);
        newSlot.transform.localScale = new Vector3(1, 1, 1);        
        activeSlots.Add(newSlot);
    }
    void PoolPush(UIFindUserSlot _uiFindUserSlot)
    {
        if (slotPool.Count >= 5)
        {
            Destroy(_uiFindUserSlot);
        }
        else
        {
            _uiFindUserSlot.transform.SetParent(poolParent.transform);
            _uiFindUserSlot.Clear();
            slotPool.Enqueue(_uiFindUserSlot);
            _uiFindUserSlot.gameObject.SetActive(false);
        }
    }
    void Clear()
    {
        foreach(UIFindUserSlot slot in activeSlots)
        {
            PoolPush(slot);
        }
    }
}
