using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    private static ResourceManager instance;
    public static ResourceManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ResourceManager>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("ResourceManager");
                    instance = obj.AddComponent<ResourceManager>();
                    DontDestroyOnLoad(instance.gameObject);
                }
            }
            return instance;
        }
    }

    public Dictionary<int, StageInfo> StageTable { get; private set; }
    public Dictionary<string,Monster> MonsterTable { get; private set; }
}
