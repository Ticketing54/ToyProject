using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FBRoomData
{
    public string Master;
    public string User1;
    public string User2;
    public string User3;
    public int count;

    public FBRoomData(string _master, string _user1="", string _user2="", string _user3="", int _count=1)
    {
        Master = _master;
        User1 = _user1;
        User2 = _user2;
        User3 = _user3;
        this.count = _count;
    }
}