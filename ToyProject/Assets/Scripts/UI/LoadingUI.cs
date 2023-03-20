using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingUI : MonoBehaviour
{   
    [SerializeField]
    Image backGround;
    [SerializeField]
    Image loadingImage;
    

    private void Update()
    {
        loadingImage.transform.Rotate(Vector3.forward * 2f);
    }
}
