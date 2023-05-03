using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Bettery : MonoBehaviour
{
    [SerializeField]
    Image level;
    [SerializeField]
    TextMeshProUGUI levelText;

    
    private void Update()
    {
        float percent = SystemInfo.batteryLevel;
        level.fillAmount = percent;
        float spercent = percent * 100;
        levelText.text = (int)spercent + " %";
        if(spercent > 50)
        {
            level.color = Color.green;
        }
        else
        {
            level.color = Color.yellow;
        }
    }
    

}
