using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class CameraManager : MonoBehaviour
{   
    public static CameraManager Instance;
    
    [SerializeField]
    CinemachineBrain brain;
    [SerializeField]
    CinemachineVirtualCamera follow;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    /// <summary>
    /// Target is Player or Castle
    /// </summary>
    /// <param name="Target"></param>
    public void TargetPlayer(GameObject _target)
    {
        follow.Priority = 20;
        follow.Follow = _target.transform;
        follow.LookAt = _target.transform;
        
    }
    public void TargetOtherPlayer(GameObject _target)
    {
        brain.enabled = true;
        // Control UI ¾ø¾Ù °Í
        brain.ActiveVirtualCamera.Follow = _target.transform;
        brain.ActiveVirtualCamera.LookAt = _target.transform;
    }
}
