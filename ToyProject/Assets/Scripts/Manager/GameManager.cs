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
    /// ���� ���� ���� (EnumŸ��)
    /// </summary>
    /// <param name="GameState"></param>
    public void ChangeGameState(GameState _state)
    {

    }

    // AuthManager �ʱ�ȭ �� ��밡�� ���� Ȯ�� , Addressable �ֽ�ȭ üũ;
    IEnumerator CoCheckUpdateAndAuth()
    {
        yield return null;

        ChangeGameState(GameState.Login);
    }
}
