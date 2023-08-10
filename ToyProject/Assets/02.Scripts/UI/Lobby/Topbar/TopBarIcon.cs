using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TopBarIcon : MonoBehaviour
{
    [SerializeField]
    protected GameObject marker;

    public abstract void OnClickButton();
    public virtual void Mark() { marker.gameObject.SetActive(true); }
    public virtual void EraseMark() { marker.gameObject.SetActive(false); }
}
