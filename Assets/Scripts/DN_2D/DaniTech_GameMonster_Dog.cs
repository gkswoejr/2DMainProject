using System.Collections;
using UnityEngine;

public class DaniTech_GameMonster_Dog : DaniTech_GameMonsterBase
{
    [Header("몬스터 프리팹에서 미리 설정할 데이터")]
    public float SkillTime = 1f;
    public GameObject Prefab_ThisMonsterSkillObject;
    public GameObject GameObject_SkillObjectRoot;
    public GameObject SpriteRenderer_ThisMonster;

    [Header("데이터를 확인할 수 있도록 임시로 열기")]
    public int _instanceId; //게임 오브젝트 매니저에서 찾는 용도 Id
    public string _dataId; //데이터드리븐용 Id
    [SerializeField] SpriteRenderer _spriteMonsterDog;

    [Header("받아왔는데 전투에서 필요한 데이터")]
    private DNMonsterData _thisMonsterData;
    public int _baseHp;
    public int _baseAttack;
    public bool _isAlive = true;
    private bool _lootRight = true;

    private Vector3 _moveDirection;

    private void OnDisable()
    {
        _isAlive = false;
    }


    public void InitMonster(int instanceId, string dataId)
    {
        _instanceId= instanceId;
        _dataId = dataId;

        var monsterData = DaniTechGameDataManager.Instance.GetDNMonsterData(dataId);
        if (monsterData != null) 
        {
            _thisMonsterData = monsterData;
            _baseHp = _thisMonsterData.BaseHp;
            _baseAttack = _thisMonsterData.BaseAttackDamage;
        }

        StartCoroutine(CheckAndUseSkill());

    }

    private int GetFinalNormalAttackDamage(int baseAttack, float normalAttackMultiple)
    {
        return GetFinalSkillAttackDamage(baseAttack,normalAttackMultiple);
    }

    private int GetFinalSkillAttackDamage(int baseAttack, float skillMultiple)
    {
        return(int)(baseAttack * skillMultiple);
    }
  
    
    

    
    //코루틴은 유니테스크로 호환이 가능하다

    IEnumerator CheckAndUseSkill()
    {
        while (_isAlive)
        {
            yield return new WaitForSeconds(SkillTime);
            if (_isAlive == false)
            {
                break;
            }
            //ChangeMonsterDirection();
            UseAttackSkill();
            
        }

    }

    private void ChangeMonsterDirection()
    {
        _lootRight = !_lootRight;
        _moveDirection = new Vector3(_lootRight ? 1 : -1, 0, 0);
        SetMeshDirectionByMoveDirection((int)_moveDirection.x);
    }

    private void SetMeshDirectionByMoveDirection(int x)
    {
        Vector3 currentScale = SpriteRenderer_ThisMonster.transform.localScale;
        currentScale.x *= -1;
        SpriteRenderer_ThisMonster.transform.localScale = currentScale;
    }

    private void UseAttackSkill()
    {
       
        var gObj = Instantiate(Prefab_ThisMonsterSkillObject, transform.position, GameObject_SkillObjectRoot.transform.rotation);
        if (gObj == null) return;
    }
}
