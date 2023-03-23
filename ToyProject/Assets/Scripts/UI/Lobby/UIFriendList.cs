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
        contentObj = GetComponent<ScrollRect>().content.gameObject;
    }

    private void OnEnable()
    {
        Reset();
        if (AuthManager.Instance.User == null)
            return;
        SettingFriendList();
    }

    void SettingFriendList()
    {
        
    }
    private void Push(UserInfo _userinfo)
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
    private void Pop(UIFriendSlot _uiFriend)
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
    /// <summary>
    /// userID 로 삭제
    /// </summary>
    /// <param name="User 고유번호 (UserID)"></param>
    private void Remove(string _userID)
    {
        
        foreach(UIFriendSlot one in friendList)
        {
            if(one.UserInfo.UserId == _userID)
            {
                one.transform.SetParent(poolParent.transform);
                one.Clear();
                Pop(one);
                friendList.Remove(one);
                break;
            }
        }
    }
    
    private void Reset()
    {
        foreach(UIFriendSlot one in friendList)
        {
            Pop(one);
        }
    }
}
