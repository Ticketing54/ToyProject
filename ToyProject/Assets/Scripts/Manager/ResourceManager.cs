using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using System.Threading.Tasks;
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
    #region Prefab
    public async Task PrefabSetting()
    {
        AsyncOperationHandle<IList<IResourceLocation>> locationHandle = Addressables.LoadResourceLocationsAsync("Prefab");
        await locationHandle.Task;
        AsyncOperationHandle<IList<GameObject>> objSetting = Addressables.LoadAssetsAsync<GameObject>(locationHandle, PrefabSetting);
        await objSetting.Task;
    }
    void PrefabSetting(GameObject _obj)
    {

    }
    #endregion

    #region Monster
    List<GameObject> runningMonster;
    IEnumerator CoSpawnMonster(int _num)
    {
        Stack<Monster> mobs = GetRoundMonsters(_num);
        while(mobs.Count  != 0)
        {
            yield return new WaitForSeconds(1f);
            // ¸÷ Á¨
            Monster mob = mobs.Pop();
            Debug.Log(mob.Atk);
        }
    }
    Stack<Monster> GetRoundMonsters(int _num)
    {
        Stack<Monster> mobs = new Stack<Monster>();
        StageInfo stageInfo = StageTable[_num];
        List<List<string>> mobinfo = stageInfo.mobInfo;
        foreach (List<string> moblist in mobinfo)
        {
            GameObject samplePrefab = prefabDic[moblist[0]];
            MonsterInfo mob = MonsterTable[moblist[0]];

            int count = int.Parse(moblist[2]);
            while (0 < count--)
            {
                GameObject monsterfrepab = Instantiate(samplePrefab);
                // ¸÷À§Ä¡ º¯°æÇÒ °Í ´ë±â Àå¼Ò·Î

                Monster monster = monsterfrepab.AddComponent<Monster>();
                monster.SettingMonster(mob.Hp, mob.Atk);
                mobs.Push(monster);
            }
        }
        return mobs;
    }
    #endregion

    /// <summary>
    /// SettingTable
    /// </summary>
    /// <param name="StageTable"></param>
    /// <param name="MonsterTable"></param>
    public void SettingTable(Dictionary<int,StageInfo> _stageTable, Dictionary<string,MonsterInfo> _mobTable)
    {
        StageTable = _stageTable;
        MonsterTable = _mobTable;
    }
    /// <summary>
    /// SettingMap (Round)
    /// </summary>
    /// <param name="RoundNumber"></param>
    public void SettingRound(int _roundNumber)
    {

    }
    public void ResetResourceManager()
    {
        StageTable = null;
        MonsterTable = null;
    }
    public Dictionary<int, StageInfo> StageTable { get; private set; }
    public Dictionary<string,MonsterInfo> MonsterTable { get; private set; }
    public Dictionary<string, GameObject> prefabDic { get; private set; }
}
