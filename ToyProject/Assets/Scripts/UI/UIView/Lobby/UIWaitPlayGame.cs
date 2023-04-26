using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
public class UIWaitPlayGame : MonoBehaviourPun
{
    [SerializeField]
    TextMeshProUGUI count;

    private void Awake()
    {
        UIManager.Instance.ACountNumber += Counting;
    }
    void Counting(int _countNumber)
    {
        if (_countNumber == -1)
        {
            gameObject.SetActive(false);
            return;
        }
        count.text = _countNumber.ToString()+ "�� �Ŀ� ���۵˴ϴ�.";
    }
    public void CancleCounting()
    {
        LobbyManager.Instance.CanclePlayGame();
    }

}
