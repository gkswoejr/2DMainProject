using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class DaniTech_GameMonster_Dog : DaniTech_GameMonsterBase
{
    private enum MonsterAIState
    {
        NONE = 0,
        IDLE,
        WALK,
        CHASE,
        ATTACK
    }
    private MonsterAIState currentAIState = MonsterAIState.WALK;

    [Header("몬스터 프리팹에서 미리 설정할 데이터")]
    public float _skillTime = 1f;
    public float walkSpeed = 1f;
    [SerializeField] private Rigidbody2D _rigidBody_Monster;
    [SerializeField] private Collider2D _collider_MonsterDetectorRadius;
    [SerializeField] private DaniTech_GameMonsterBase_Collider _daniTech_GameMonsterBase_Collider;


    private float _horizontalInput;
    private bool _lookRight = true;
    private bool isMoving = false;
    private Transform _monsterTargetTranform;
    public GameObject GameObject_SkillObjectRoot;
    public GameObject SpriteRenderer_ThisMonster;
    [SerializeField] private string id_SkillObject;

    [Header("움직일 타겟 포지션")]
    private Vector3 targetPosition;
    private Vector3 moveDirection;
    private float currentXVelocity;

    [Header("애니메이터")]
    [SerializeField] private DaniTech_2DAnimatorController AnimatorController_Entity;

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


    private void OnEnable()
    {

    }
    private void OnDisable()
    {
        _isAlive = false;
        ResetstatChangedEvent(); //스탯변경하는 이벤트 초기화
        ResetMonsterDetectorRadius();
    }

    private void Update()
    {
        //Vector3 moveDirection = Vector3.zero;



        currentXVelocity = _rigidBody_Monster.linearVelocity.x;



        // 3. 캐릭터 방향 전환 (Flip)
        if (currentXVelocity > 0.03f && !_lookRight)
        {
            Flip();
        }
        else if (currentXVelocity < -0.03f && _lookRight)
        {
            Flip();
        }

        bool isMoving = currentXVelocity > 0.01f;
        //ChangeMonsterState(isMoving ? DaniTech_EntityAnimState.Walk : DaniTech_EntityAnimState.Idle);


    }

    void FixedUpdate()
    {
        Move();

    }
    void Move()
    {
        // Y축 속도는 유지하면서 X축 속도만 변경 (관성 유지)
        _rigidBody_Monster.linearVelocity = new Vector2(moveDirection.x * walkSpeed, _rigidBody_Monster.linearVelocity.y);

    }

    void Flip()
    {
        _lookRight = !_lookRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }
    private void ChangeMonsterState(DaniTech_EntityAnimState newState)
    {
        // 이런 곳에 UI나 플레이어의 별도 처리를 넣어줄 수도 있다


        // 우선 애니메이션만 바꿔 봅시다
        AnimatorController_Entity.SetState(newState);
    }



    public void InitMonster(int instanceId, string dataId)
    {
        _instanceId = instanceId;
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

        //StartCoroutine(CheckAndUseSkill());

        StartCoroutine(CheckAndWalk());

        BindMonsterDetectorOnTrigger2D();
    }


    public int GetMonsterInstanceId() // 유니티에 GetInstanceID랑 헷갈리지 않도록 함수명을 복잡하게 쓴다
    {
        // 객체 - 데이터 부에 있는것을 반환
        return _instanceId;
    }
    private int GetFinalNormalAttackDamage(int baseAttack, float normalAttackMultiple)
    {
        return GetFinalSkillAttackDamage(baseAttack, normalAttackMultiple);
    }

    private int GetFinalSkillAttackDamage(int baseAttack, float skillMultiple)
    {
        return (int)(baseAttack * skillMultiple);
    }





    //코루틴은 유니테스크로 호환이 가능하다

    IEnumerator CheckAndUseSkill()
    {
        while (_isAlive)
        {

            yield return new WaitForSeconds(_skillTime);
            if (_isAlive == false)
            {
                break;
            }
            //ChangeMonsterDirection();
            if (currentAIState == MonsterAIState.CHASE)
            {
                UseAttackSkill();
            }

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

        var tag = this.gameObject.tag;

        // 람다식을 이용해 생성 완료 후 gObj와 "똑같은 작용"을 하도록 로직을 넘깁니다.
        DaniTechGameObjectManager.Inst.CreateSKillObject(id_SkillObject, GameObject_SkillObjectRoot.transform, (createdObj) =>
        {
            if (createdObj == null) return;

            // gObj에서 하던 컴포넌트 세팅 및 초기화를 그대로 수행
            var skillProjectileComponent = createdObj.GetComponent<HJD_SkillBase>();
            if (skillProjectileComponent == null) return;

            // TODO : 추후 함수로 빠져야함
            float skillMultiple = _thisMonsterData.SkillAttackMultipleList.Count > 0 ? _thisMonsterData.SkillAttackMultipleList[0] : 0;
            int finalSkillDamage = GetFinalSkillAttackDamage(_baseAttack, skillMultiple);

            skillProjectileComponent.InitSkillObject(_instanceId, GameObject_SkillObjectRoot.transform.position, finalSkillDamage, tag, OnSkillCollision);
        }).Forget();
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

    private void BindMonsterDetectorOnTrigger2D()
    {
        _daniTech_GameMonsterBase_Collider.OnTriggerStayEvent += PlayerDetected;
        _daniTech_GameMonsterBase_Collider.OnTriggerEnterEvent += PlayerFind;
        _daniTech_GameMonsterBase_Collider.OnTriggerExitEvent += PlayerLost;
    }



    private void ResetMonsterDetectorRadius()
    {
        _daniTech_GameMonsterBase_Collider.OnTriggerStayEvent -= PlayerDetected;
        _daniTech_GameMonsterBase_Collider.OnTriggerEnterEvent -= PlayerFind;
        _daniTech_GameMonsterBase_Collider.OnTriggerExitEvent -= PlayerLost;

    }



    private void PlayerDetected(Collider2D playerCollider)
    {
        if (currentAIState != MonsterAIState.CHASE)
        {
            ChangeMonsterAIStateToCHASE();
        }

        _monsterTargetTranform = playerCollider.transform;

        moveDirection = (_monsterTargetTranform.position - transform.position).normalized;


        if (moveDirection != Vector3.zero)
        {
            // 3. 해당 방향을 바라보는 회전값 생성
            float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
            // 4. 즉시 회전시키고 싶다면:
            GameObject_SkillObjectRoot.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        }
    }

    private void PlayerFind(Collider2D playerCollider)
    {
        _monsterTargetTranform = playerCollider.transform;
        ChangeMonsterAIStateToCHASE();

    }
    private void PlayerLost(Collider2D playerCollider)
    {
        // 타겟 초기화 (추적 종료 및 다시 배회 모드로 전환)
        if (_monsterTargetTranform != null && playerCollider.transform == _monsterTargetTranform)
        {
            _monsterTargetTranform = null;
            ChangeMonsterAIStateToWALK();
        }
    }

    IEnumerator CheckAndWalk()
    {


        while (_isAlive)
        {
           

            switch (currentAIState)
            {

                case MonsterAIState.CHASE:

                    UseAttackSkill();
                    yield return new WaitForSeconds(_skillTime);
                    break;

                case MonsterAIState.WALK:

                    ChangeMonsterAIStateToIDLE();

                    float dir = Random.Range(0, 2) == 0 ? -1f : 1f;
                    moveDirection = new Vector3(dir, 0, 0);
                    moveDirection.Normalize();

                    float walkTime = Random.Range(1f, 2f);
                    yield return new WaitForSeconds(walkTime);

                    // 걷는 동안 플레이어가 감지되어 CHASE 등으로 바뀌었다면 상태 전환을 취소하고 루프 처음으로
                    if (currentAIState != MonsterAIState.WALK) continue;

                    break;

                case MonsterAIState.IDLE:

                    ChangeMonsterAIStateToWALK();

                    moveDirection = Vector3.zero;

                    float waitTime = Random.Range(1f, 2f);
                    yield return new WaitForSeconds(waitTime);

                    // 쉬는 동안 플레이어가 감지되어 상태가 바뀌었다면 루프 처음으로
                    if (currentAIState != MonsterAIState.IDLE) continue;

                    break;
                default:
                    yield return null;
                    break;
            }
            yield return null;
        }
    }


    private void InvokeStatChangedEvent()
    {
        _onHpChanged?.Invoke(_baseHp, _maxHp);
        // _onSpChanged?.Invoke(_playerSp);
    }

    private void ChangeMonsterAIStateToIDLE()
    {
        currentAIState = MonsterAIState.IDLE;
        Debug.Log(MonsterAIState.IDLE);
        moveDirectionToZero();
    }
    private void ChangeMonsterAIStateToWALK()
    {
        currentAIState = MonsterAIState.WALK;
        Debug.Log(MonsterAIState.WALK);

    }
    private void ChangeMonsterAIStateToCHASE()
    {
        currentAIState = MonsterAIState.CHASE;
        Debug.Log(MonsterAIState.CHASE);

    }

    private void moveDirectionToZero()
    {
        if (currentAIState == MonsterAIState.IDLE)
        {
            moveDirection = Vector3.zero;
        }
    }
}
