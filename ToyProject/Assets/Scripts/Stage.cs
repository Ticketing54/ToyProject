using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageInfo 
{   
    public int gold;
    public int exp;
    public List<List<string>> mobInfo;
    public StageInfo(int _gold,int _exp,List<List<string>> _mobinfo)
    {
        gold = _gold;
        exp = _exp;
        mobInfo = _mobinfo;
    }
}
