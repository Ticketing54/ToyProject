using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private static InputManager instance;
    public static InputManager Instance
    {
        get
        {
            instance = FindObjectOfType<InputManager>();
            if (instance == null)
            {
                GameObject obj = new GameObject("LobbyManager");
                instance = obj.AddComponent<InputManager>();
                DontDestroyOnLoad(instance.gameObject);
            }

            return instance;
        }
    }
    public delegate Vector3 direction ();
    public ControlUnit Player { get; private set; }
    Coroutine control;
    /// <summary>
    /// ĳ���� ����
    /// </summary>
    /// <param name="Unit"></param>
    public void SetUnit(ControlUnit _unit) { Player = _unit; }
    /// <summary>
    /// ĳ���� ��Ʈ�� ����
    /// </summary>
    public void StartControl(direction _dir) { control = StartCoroutine(CoControlUnit(_dir)); }
    /// <summary>
    /// ��� ���� �ʱ�ȭ
    /// </summary>
    public void Clear()
    {
        StopCoroutine(control);
        Player = null;
    }
    /// <summary>
    /// Control
    /// </summary>
    /// <returns></returns>
    IEnumerator CoControlUnit(direction _dir)
    {
        while(true)
        {
            yield return null;

            if (Player == null)
            {
                continue;
            }
            Player.gameObject.transform.position = Player.Speed * _dir() * Time.deltaTime;
        }
    }
    



}
