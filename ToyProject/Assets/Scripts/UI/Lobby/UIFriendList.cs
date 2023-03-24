using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIFriendList : MonoBehaviour
{   
    // Sample
    [SerializeField]
    UIFriendSlot sampleUiFriend;
    [SerializeField]
    GameObject poolParent;
    
    List<UIFriendSlot> friendList;
    Queue<UIFriendSlot> uiFriendPool;
    GameObject contentObj;

    private void Awake()
    {
        friendList = new();
        uiFriendPool = new();
        
    }

    private void OnEnable()
    {
        //Clear();
        //SettingFriendList();
    }

    void SettingFriendList()
    {
        
    }
    private void Add(UserInfo _userinfo)
    {
        UIFriendSlot newfriend = uiFriendPool.Dequeue();
        if (newfriend == null)
        {
            newfriend = Instantiate<UIFriendSlot>(sampleUiFriend);
        }
        newfriend.gameObject.SetActive(true);
        newfriend.transform.localScale = new Vector3(1, 1, 1);
        newfriend.transform.SetParent(contentObj.transform);
        friendList.Add(newfriend);
    }
    private void PoolPush(UIFriendSlot _uiFriend)
    {
        if(uiFriendPool.Count>=5)
        {
            Destroy(_uiFriend);
        }
        else
        {
            _uiFriend.transform.SetParent(poolParent.transform);
            _uiFriend.Clear();
            uiFriendPool.Enqueue(_uiFriend);
            _uiFriend.gameObject.SetActive(false);
        }
    }

    private void Clear()
    {
        foreach(UIFriendSlot one in friendList)
        {
            PoolPush(one);
        }
    }
}
