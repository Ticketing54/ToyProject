using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CameraManager : MonoBehaviour
{   
    public static CameraManager Instance;
    public Vector3 Preset = new Vector3(0, 8, -10);
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }
    /// <summary>
    /// Folling Camera
    /// </summary>
    /// <param name="Target"></param>
    public void SetActiveCamera(GameObject _target) { StartCoroutine(CoActiveCamera(_target)); }
    IEnumerator CoActiveCamera(GameObject _target)
    {
        transform.rotation = Quaternion.Euler(35f, 0, 0);
        while(true)
        {
            if (_target == null)
                break;
            transform.position = _target.transform.position + Preset;
            yield return null;
        }
    }
}
