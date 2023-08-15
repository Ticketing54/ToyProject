using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

public abstract class UIView : MonoBehaviour
{
    [SerializeField] protected VISIBLESTATE state;
    public VISIBLESTATE State { get => state; }

    public enum VISIBLESTATE
    {
        APPEARING,
        APPEARED,
        DISAPPEARING,
        DISAPPEARED,
    }
}
