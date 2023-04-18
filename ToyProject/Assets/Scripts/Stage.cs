using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageInfo : MonoBehaviour
{   
    int gold;
    int exp;
    List<List<string>> mobInfo;
    public StageInfo(int _gold,int _exp,List<List<string>> _mobinfo)
    {
        gold = _gold;
        exp = _exp;
        mobInfo = _mobinfo;
    }
}
