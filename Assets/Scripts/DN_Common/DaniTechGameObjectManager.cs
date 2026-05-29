using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class DaniTechGameObjectManager : MonoBehaviour
{
    // 생성할 몬스터의 프리팹
    [SerializeField] private GameObject Prefab_Enemy;
    [SerializeField] private Transform Root_Enemy;

    //생성할 스킬 프리팹
    [SerializeField] private GameObject _prefab_Skill;
    [SerializeField] private Transform _root_Skill;

    public static DaniTechGameObjectManager Inst { get; set; }

    // 생성된 오브젝트의 키가 됨
    private int _objectInstanceKeyGenerator = 0;

    // 생성된 오브젝트의 생명을 보관
    private Dictionary<int, GameObject> _createdGameObjectContainer = new Dictionary<int, GameObject>();
    private Dictionary<int, DaniTech_2DFieldObject> _fieldObjectContainer = new Dictionary<int, DaniTech_2DFieldObject>();
    private Dictionary<int, DaniTech_GameMonster_Dog> _gameMonsterObjectContainer = new Dictionary<int, DaniTech_GameMonster_Dog>();

    private DaniTech_2DPlayer _localPlayer;

   
    private void Awake()
    {
        Inst = this;
    }

    public void RegisterLocalPlayer(DaniTech_2DPlayer localPlayer)
    {
        _localPlayer = localPlayer;
    }
    //프로퍼티가 있긴 하지만 직접 참조하는 것보다는 Get 함수를 한정해서 사용하자


    public DaniTech_2DPlayer GetLocalPlayer()
    {
        if (_localPlayer == null)
        {
            Debug.LogError("등록된 플레이어가 없는데! 참조하려고 시도하고 있습니다!!");
            return null;
        }

        // 우리가 배웠던 원시적인 Get함수입니다. -> 원시적이지만 유용함
        return _localPlayer;
    }


    public void RequestSpawnEnemy()
    {
        if (Prefab_Enemy == null)
        {
            Debug.LogWarning("프리팹이 등록되지 않은 오브젝트 입니다.");
            return;
        }

        var gObj = Instantiate(Prefab_Enemy, Root_Enemy);
        if (gObj == null)
        {
            Debug.LogWarning("생성에 실패한 게임 오브젝트 입니다.");
            return;
        }

        // 1-1 생성에 성공했다면, 미리 Key를 발급한다.
        _objectInstanceKeyGenerator++;

        // 1-2 Dictionary에 추가하기 전에 미리 키 검사한다
        if (_createdGameObjectContainer.ContainsKey(_objectInstanceKeyGenerator) == true)
        {
            Debug.LogWarning("이미 동일한 키가 발급된 게임 오브젝트가 존재합니다");
            return;
        }

        // 1-3 동적생성(실체화)된 오브젝트를 게임 오브젝트 매니저의 자료구조(Dictionary)에 보관하자!
        _createdGameObjectContainer.Add(_objectInstanceKeyGenerator, gObj);
        InitGeneratedEntityObject(_objectInstanceKeyGenerator, gObj);

        Debug.Log($"키: {_objectInstanceKeyGenerator}의 객체 {gObj.name}이 호출되었습니다.");
    }

    private void InitGeneratedEntityObject(int generatedId, GameObject gObj)
    {
        // 4-1 지금은 Enemy지만, 나중에 IGameEntity 같은 인터페이스로 개선하면 더 좋다
        DaniTech_2DEnemy gameEntity = gObj.GetComponent<DaniTech_2DEnemy>();
        if (gameEntity == null)
        {
            Debug.LogWarning($"생성된 {gObj.name}의 InstanceId를 대입할 수 있는 컴포넌트를 가져올 수 없습니다!");
            return;
        }

        // 4-2 생성된 객체에 정보를 부여한다!
        gameEntity.InitEnemyInfo(generatedId);
    }


    public GameObject GetEntityObjectCanBeNull(int instanceId)
    {
        if (_createdGameObjectContainer.ContainsKey(instanceId) == false)
        {
            Debug.LogWarning($"{instanceId}는 존재하지 않습니다.");
            return null;
        }

        // 2-1 실체화하면서 등록된 게임 오브젝트가 있다면 반환
        return _createdGameObjectContainer[instanceId];
    }

    public void RequestDestroyEntityObject(int instanceId)
    {
        var gObj = GetEntityObjectCanBeNull(instanceId);
        if (gObj == null)
        {
            return;
        }

        // 3-1 요청된 객체를 제거함
        _createdGameObjectContainer.Remove(instanceId);
        Destroy(gObj);
    }

    public async UniTaskVoid CreateMonsterObject(string monsterObjectDataId, Transform spawnSpot)
    {
        var monsterObject = DaniTechGameDataManager.Instance.GetDNMonsterData(monsterObjectDataId);
        if (monsterObject == null) return;

        var createdObj = await DaniTechResourceManager.Inst.InstantiateAsync(monsterObject.PrefabPath, Root_Enemy, true);
        createdObj.transform.position = spawnSpot.position;
        


        AddMonsterObjectOnCreate(createdObj, monsterObjectDataId);

    }

    private void AddMonsterObjectOnCreate(GameObject createdObject, string monsterDataId)
    {
        _objectInstanceKeyGenerator++;
        int generatedInstanceId = _objectInstanceKeyGenerator;

        var monsterComponent = createdObject.GetComponent<DaniTech_GameMonster_Dog>();
        if (monsterComponent == null) return;

        _gameMonsterObjectContainer.Add(generatedInstanceId, monsterComponent);

        monsterComponent.InitMonster(generatedInstanceId, monsterDataId);
    }

    public DaniTech_GameMonster_Dog GetMonsterObjectByInstanceId(int monsterInstanceId)
    {
        if (_gameMonsterObjectContainer.ContainsKey(monsterInstanceId) == false)
        {
            Debug.LogError($"{monsterInstanceId} 찾으려는 몬스터가 유효하지 않습니다");
            return null;
        }

        return _gameMonsterObjectContainer[monsterInstanceId];
    }


    //[필드 오브젝트] ====================================================================================================

    public async UniTaskVoid CreateFieldObject(string fieldObjectDataId, Transform spawnSpot)
    {
        var fieldObject = DaniTechGameDataManager.Instance.GetDNFieldObjectData(fieldObjectDataId);
        if (fieldObject != null)
        {
            var createdObj = await DaniTechResourceManager.Inst.InstantiateAsync(fieldObject.PrefabPath, Root_Enemy, true);
            createdObj.transform.position = spawnSpot.position;
            AddFieldObjectOnCreate(createdObj, fieldObjectDataId);
        }
    }

    public async UniTaskVoid CreateSKillObject(string skillObjectDataId, Transform spawnSpot, System.Action<GameObject> onCreated = null)
    {
        var skillObject = DaniTechGameDataManager.Instance.GetSkill(skillObjectDataId);
        if (skillObject != null)
        {
            var createdObj = await DaniTechResourceManager.Inst.InstantiateAsync(skillObject.PrefabPath, _root_Skill, true);
            createdObj.transform.position = spawnSpot.position;
            createdObj.transform.rotation = spawnSpot.rotation;



            //AddFieldObjectOnCreate(createdObj, skillObjectDataId);
            onCreated?.Invoke(createdObj);
        }
    }

    private void AddFieldObjectOnCreate(GameObject createdObject, string fieldObjectDataId)
    {
        _objectInstanceKeyGenerator++;
        var generatedInstanceId = _objectInstanceKeyGenerator;
        var fieldObject = createdObject.GetComponent<DaniTech_2DFieldObject>();

        if (fieldObject != null)
        {
            _fieldObjectContainer.Add(generatedInstanceId, fieldObject);
            fieldObject.InitFieldObjectInfoOnCreated(generatedInstanceId, fieldObjectDataId);
        }
    }

    public void RequestDestroyFieldObject(int instanceId)
    {
        var fieldObjectComponent = GetFieldObjectByInstanceId(instanceId);
        if (fieldObjectComponent == null)
        {
            return;
        }

        // 요청된 필드 오브젝트를 제거함
        _fieldObjectContainer.Remove(instanceId);
        Destroy(fieldObjectComponent.gameObject);
    }

    public DaniTech_2DFieldObject GetFieldObjectByInstanceId(int fieldObjectInstanceId)
    {
        if (_fieldObjectContainer.ContainsKey(fieldObjectInstanceId) == false)
        {
            Debug.LogError($"{fieldObjectInstanceId} 찾으려는 필드 오브젝트가 유효하지 않습니다");
            return null;
        }

        return _fieldObjectContainer[fieldObjectInstanceId];
    }
}
