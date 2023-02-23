using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        StartCoroutine(CoCheckUpdateAndAuth());
    }

    /// <summary>
    /// 게임 상태 변경 (Enum타입)
    /// </summary>
    /// <param name="GameState"></param>
    public void ChangeGameState(GameState _state)
    {

    }

    // AuthManager 초기화 및 사용가능 여부 확인 , Addressable 최신화 체크;
    IEnumerator CoCheckUpdateAndAuth()
    {
        yield return null;

        ChangeGameState(GameState.Login);
    }
}
