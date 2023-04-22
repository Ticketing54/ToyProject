using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class CameraManager : MonoBehaviour
{
    [SerializeField]
    CinemachineBrain brain;
    [SerializeField]
    CinemachineVirtualCamera mainCamera;
    [SerializeField]
    CinemachineVirtualCamera otherCamera;


    [SerializeField]
    GameObject player;
    

    
    /// <summary>
    /// Target is Player or Castle
    /// </summary>
    /// <param name="Target"></param>
    public void TargetOn(GameObject _target)
    {
        
    }
    public void Test()
    {
        
    }
    

    public void Test2()
    {

    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.S))
        {
            TargetOn(player);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            TargetOn(player);
        }
    }
}
