using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIFriendList : MonoBehaviour
{   
    [SerializeField]
    UIFriendSlot sampleUiFriend;
    [SerializeField]
    GameObject poolParent;
    [SerializeField]
    GameObject contentObj;

    HashSet<UIFriendSlot> friendList;
    ObjectPool<UIFriendSlot> pool;
    

    private void Awake()
    {
        friendList = new();
        pool = new ObjectPool<UIFriendSlot>(sampleUiFriend, poolParent.transform);
        
    }

    private void OnEnable()
    {
        Clear();
        UIManager.uiManager.AFriendListClear += Clear;
        UIManager.uiManager.AFriendAdd += Add;
        AuthManager.Instance.UpdateFriendList();
    }
    private void OnDisable()
    {
        UIManager.uiManager.AFriendListClear -= Clear;
        UIManager.uiManager.AFriendAdd -= Add;
    }

    private void Add(UserInfo _userinfo)
    {
        UIFriendSlot newfriend = pool.Get();
        newfriend.gameObject.SetActive(true);        
        newfriend.transform.SetParent(contentObj.transform);
        if(_userinfo.Connect == true)
        {
            newfriend.transform.SetAsLastSibling();
        }
        newfriend.transform.localScale = new Vector3(1, 1, 1);
        newfriend.SetProfile(_userinfo);
        friendList.Add(newfriend);
    }
    private void PoolPush(UIFriendSlot _uiFriend)
    {
        _uiFriend.Clear();
        pool.Push(_uiFriend);
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
