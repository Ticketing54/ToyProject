using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Character : MonoBehaviourPun
{
    public Vector3 Direction = Vector3.zero;
    public float Hp_Max;
    public float Hp_Cur;
    public float Atk;
    public float Speed;
    Animator anim;
    public GameObject cameraSet;
    public void SettingCharacter()
    {
        anim = GetComponent<Animator>();
        cameraSet = transform.Find("Camera").gameObject;
        Speed = 2f;

        LevelSetting(1);
    }
    public void Attack()
    {
        anim.SetTrigger("Attack");
    }
    /// <summary>
    /// Table
    /// </summary>
    void LevelSetting(int _level)
    {

    }
}
