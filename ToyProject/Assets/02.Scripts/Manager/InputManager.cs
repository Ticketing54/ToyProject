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
    public Character Player { get; private set; }
    /// <summary>
    /// ĳ���� ����
    /// </summary>
    /// <param name="Unit"></param>
    public void SetUnit(Character _unit) { Player = _unit; }
    /// <summary>
    /// ĳ���� ��Ʈ�� ����
    /// </summary>
    public void Control(Vector3 _dir)
    {
        if (Player == null)
            return;
        Player.Direction = _dir;
    }
    public void NomalAttack()
    {
        if (Player == null)
            return;
        Player.NomalAttack();
    }
    /// <summary>
    /// ��� ���� �ʱ�ȭ
    /// </summary>
    public void Clear()
    {   
        Player = null;
    }
    
    



}