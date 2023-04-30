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
    public void SettingCharacter(int _photonViewIDNumber)
    {
        anim = GetComponent<Animator>();
        Speed = 4f;
        LevelSetting(1);
        PhotonView pv = PhotonView.Get(this);
        pv.ViewID = _photonViewIDNumber;
    }
    public void NomalAttack()
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
