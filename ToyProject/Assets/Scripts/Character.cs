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
    public PhotonView photonview;
    public void SettingCharacter(PhotonView _pv)
    {
        anim = GetComponent<Animator>();
        Speed = 4f;
        photonview = _pv;
        photonView.ObservedComponents = new List<Component>();
        photonView.ObservedComponents.Add(GetComponent<PhotonAnimatorView>());
        photonView.ObservedComponents.Add(GetComponent<PhotonTransformView>());
        LevelSetting(1);
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
