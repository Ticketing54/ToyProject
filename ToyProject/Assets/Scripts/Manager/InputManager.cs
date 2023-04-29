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
    /// 캐릭터 세팅
    /// </summary>
    /// <param name="Unit"></param>
    public void SetUnit(Character _unit) { Player = _unit; }
    /// <summary>
    /// 캐릭터 컨트롤 시작
    /// </summary>
    public void Control(Vector3 _dir)
    {
        if (Player == null)
            return;
        Player.Direction = _dir;
    }
    /// <summary>
    /// 모든 정보 초기화
    /// </summary>
    public void Clear()
    {   
        Player = null;
    }
    
    



}
