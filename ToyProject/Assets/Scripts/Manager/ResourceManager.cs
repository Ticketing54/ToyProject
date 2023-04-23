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
        prefabDic = new Dictionary<string, GameObject>();
        AsyncOperationHandle<IList<GameObject>> objSetting = Addressables.LoadAssetsAsync<GameObject>("Prefab",
            (obj)=> 
            {
                if(!prefabDic.ContainsKey(obj.name))
                {
                    prefabDic.Add(obj.name, obj);
                }
                else
                {
                    Debug.Log(obj.name + " 은 이미 등록되어있습니다.");
                }
            });
        await objSetting.Task;
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
            // 몹 젠
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
                // 몹위치 변경할 것 대기 장소로

                Monster monster = monsterfrepab.AddComponent<Monster>();
                monster.SettingMonster(mob.Hp, mob.Atk);
                mobs.Push(monster);
            }
        }
        return mobs;
    }
    #endregion

    public void SettingMap(int _playerCount)
    {
        PlayerCount = _playerCount;
        spawnPosition = new List<Vector3>();
        GameObject[] spawner =GameObject.FindGameObjectsWithTag("Spawner");
        // castle Script 작성

        for(int i=0;i<_playerCount;i++)
        {
            spawnPosition.Add(spawner[i].transform.position);
        }

        foreach(Vector3 pos in spawnPosition)
        {
            GameObject character = Instantiate(prefabDic["Knight"]); // character스크립트에서  realease 구현할 것
            // 캐릭터 스크립트 addcomponent
            character.transform.localScale = new Vector3(1f, 1f, 1f);
            character.transform.position = pos;
        }

        GameObject castle = Instantiate(prefabDic["Castle"]);
        castle.transform.tag = "Castle";
        // castle 스크립트 addComponent
        castle.transform.position = new Vector3(75, 0, 75);
    }
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
        prefabDic = null;
        PlayerCount = 0;
    }
    public Dictionary<int, StageInfo> StageTable { get; private set; }
    public Dictionary<string,MonsterInfo> MonsterTable { get; private set; }
    public Dictionary<string, GameObject> prefabDic { get; private set; }
    public int PlayerCount { get; private set; }

    private List<Vector3> spawnPosition;
}
