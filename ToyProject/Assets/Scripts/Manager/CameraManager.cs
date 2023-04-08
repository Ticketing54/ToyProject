using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField]
    GameObject player;

    Vector3 rotateOffset;
    Vector3 vectorOffset;
    private void Start()
    {
        rotateOffset= new Vector3(40f, 0f, 0f);
        vectorOffset = new Vector3(0f, 10f, -10f);
        transform.rotation = Quaternion.Euler(rotateOffset);
    }

    void Update()
    {
        transform.position = player.transform.position + vectorOffset;
    }
}
