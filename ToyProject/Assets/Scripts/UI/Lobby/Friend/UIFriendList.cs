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
    [SerializeField]
    GameObject contentObj;

    HashSet<UIFriendSlot> friendList;
    Queue<UIFriendSlot> uiFriendPool;
    

    private void Awake()
    {
        friendList = new();
        uiFriendPool = new();
        
    }

    private void OnEnable()
    {
        Clear();
        AuthManager.Instance.AFriendListClear += Clear;
        AuthManager.Instance.AFriendAdd += Add;
        AuthManager.Instance.UpdateFriendList();
    }
    private void OnDisable()
    {
        AuthManager.Instance.AFriendListClear -= Clear;
        AuthManager.Instance.AFriendAdd -= Add;
    }

    private void Add(UserInfo _userinfo)
    {
        UIFriendSlot newfriend = null;
        if(uiFriendPool.Count != 0)
        {
            newfriend = uiFriendPool.Dequeue();
        }
        else
        {
            newfriend = Instantiate<UIFriendSlot>(sampleUiFriend);
        }
        newfriend.gameObject.SetActive(true);        
        newfriend.transform.SetParent(contentObj.transform);
        newfriend.transform.localScale = new Vector3(1, 1, 1);
        newfriend.SetProfile(_userinfo);
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
        friendList.Clear();
    }
}
