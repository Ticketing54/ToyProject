using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class UIPlayCount : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI text;
    private void OnEnable()
    {
        text.text = "";
        UIManager.Instance.ATimer += SettingCount;
    }
    private void OnDisable()
    {
        UIManager.Instance.ATimer -= SettingCount;
    }
    void SettingCount (float _count)
    {
        if(_count == 0)
        {
            text.text = "";
        }
        else
        {
            text.text = "�����ð� " + _count.ToString("F1") + " ��";
        }
    }
}
