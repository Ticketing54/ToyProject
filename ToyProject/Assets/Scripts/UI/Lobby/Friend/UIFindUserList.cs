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
    ObjectPool<UIFindUserSlot> pool;
    
    private void Awake()
    {
        activeSlots = new List<UIFindUserSlot>();
        pool = new ObjectPool<UIFindUserSlot>(sampleSlot, poolParent.transform);

    }
    private void OnEnable()
    {
        Clear();
    }
    
    public void OnClickFindButton()
    {   
        if (nickNameintput.text == "")
            return;
        Clear();
        string nicName = nickNameintput.text;
        nickNameintput.text = "";
        AuthManager.Instance.FindUserList_NickName(nicName,Add);
    }
    
    void Add(UserInfo _userinfo)
    {
        UIFindUserSlot newSlot = pool.Get();
        newSlot.gameObject.SetActive(true);
        newSlot.SetProfile(_userinfo);
        newSlot.transform.SetParent(contents.transform);
        newSlot.transform.localScale = new Vector3(1, 1, 1);        
        activeSlots.Add(newSlot);
    }
    void PoolPush(UIFindUserSlot _uiFindUserSlot)
    {
        _uiFindUserSlot.Clear();
        pool.Push(_uiFindUserSlot);
    }
    void Clear()
    {
        foreach(UIFindUserSlot slot in activeSlots)
        {
            PoolPush(slot);
        }
        activeSlots.Clear();
    }
}
