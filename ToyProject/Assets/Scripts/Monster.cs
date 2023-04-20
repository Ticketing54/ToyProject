using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public float Hp { get; private set; }
    public float Atk { get; private set; }
    public void SettingMonster(float _hp,float _atk)
    {
        Hp = _hp;
        Atk = _atk;
    }
}
