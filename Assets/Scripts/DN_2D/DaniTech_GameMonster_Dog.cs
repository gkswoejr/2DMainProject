using System;
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
    private int _maxHp;

    private Vector3 _moveDirection;

    private event Action<int, int> _onHpChanged;
    private event Action<int, int> _onSpChanged;

    private void OnDisable()
    {
        _isAlive = false;
        ResetstatChangedEvent(); //스탯변경하는 이벤트 초기화
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

            _maxHp = _baseHp;
        }

        DaniTechUIManager.Instance.AddHudSlot(instanceId, this.gameObject.transform);

        StartCoroutine(CheckAndUseSkill());

    }


    public int GetMonsterInstanceId() // 유니티에 GetInstanceID랑 헷갈리지 않도록 함수명을 복잡하게 쓴다
    {
        // 객체 - 데이터 부에 있는것을 반환
        return _instanceId;
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

        var skillProjectileComponent = gObj.GetComponent<HJD_SkillProjectile>();
        if (skillProjectileComponent == null) return;

        // TODO : 추후 함수로 빠져야함

        float skillMultiple = _thisMonsterData.SkillAttackMultipleList.Count > 0 ? _thisMonsterData.SkillAttackMultipleList[0] : 0;
        int finalSkillDamage = GetFinalSkillAttackDamage(_baseAttack, skillMultiple);
        
        var tag = this.gameObject.tag;
        skillProjectileComponent.InitSkillObject(_instanceId, this.transform.position, finalSkillDamage, tag, OnSkillCollision);
    }

    private void OnSkillCollision(int colliedObjectInstanceId, int damage)
    {
        if (colliedObjectInstanceId == 0) // 0이면 플레이어라는 규칙이 있으므로
        {
            var player = DaniTechGameObjectManager.Inst.GetLocalPlayer();

            // 스킬이 충돌한 시점에서 다시한번 데미지를 계산해도 된다 - 기획적인 요소
            // float skillMultiple = _thisMonsterData.SkillAtkMultipleList.Count > 0 ? _thisMonsterData.SkillAtkMultipleList[0] : 0;
            // int finalSkillDamage = GetFinalSkillDamage(_baseAtk, skillMultiple);

            player.TakeDamage(damage);
        }
    }

    public void TakeDamage(int playerDamage)
    {
        _baseHp -= playerDamage;

        // 피격 이펙트 같은거 활성화
        // SpriteRenderer_Damage.gameObject.SetActive(true);

        // 몬스터 죽음
        InvokeStatChangedEvent();
        if (_baseHp < 0)
        {
            Destroy(this.gameObject);
            DaniTechUIManager.Instance.RemoveHudSlot(_instanceId);
        }
    }


    public void BindOnstatChangedEvent(Action<int, int> hpChangeCallback, Action<int, int> spChangeCallback)
    {
        _onHpChanged += hpChangeCallback;
        _onSpChanged += spChangeCallback;
    }
    public void ResetstatChangedEvent()
    {
        _onHpChanged = null;
        _onSpChanged = null;
    }

    private void InvokeStatChangedEvent()
    {
        _onHpChanged?.Invoke(_baseHp, _maxHp);
        // _onSpChanged?.Invoke(_playerSp);
    }

}
