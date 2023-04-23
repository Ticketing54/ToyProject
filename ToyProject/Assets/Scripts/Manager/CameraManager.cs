using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class CameraManager : MonoBehaviour
{   
    private static CameraManager Instance;
    
    [SerializeField]
    CinemachineBrain brain;
    [SerializeField]
    GameObject player;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        brain = GetComponent<CinemachineBrain>();
        brain.enabled = false;
    }

    /// <summary>
    /// Target is Player or Castle
    /// </summary>
    /// <param name="Target"></param>
    public void TargetPlayer(GameObject _target)
    {
        brain.enabled = true;
        // Control UI 로 돌릴것
        brain.ActiveVirtualCamera.Follow = _target.transform;
        brain.ActiveVirtualCamera.LookAt = _target.transform;
    }
    public void TargetOtherPlayer(GameObject _target)
    {
        brain.enabled = true;
        // Control UI 없앨 것
        brain.ActiveVirtualCamera.Follow = _target.transform;
        brain.ActiveVirtualCamera.LookAt = _target.transform;
    }
}
