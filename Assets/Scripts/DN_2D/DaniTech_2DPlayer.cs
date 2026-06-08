using System;
using System.Collections;
using UnityEngine;

// +) 어떤 컴포넌트가 필수로 필요하다는 것을 강제할 수 있다
[RequireComponent(typeof(Rigidbody2D))]
public class DaniTech_2DPlayer : MonoBehaviour
{
    [Header("이동 설정")]
    [SerializeField] private float _moveSpeed = 8f;
    [SerializeField] private float _jumpForce = 12f;

    [Header("지면 체크 설정")]
    [SerializeField] private Transform _groundCheck;    // 발 밑에 배치할 빈 오브젝트
    [SerializeField] private float _checkRadius = 0.5f; // 체크 범위
    [SerializeField] private LayerMask _groundLayer;    // 지면으로 인식할 레이어 (Platforms 등)

    [Header("애니메이터")]
    [SerializeField] private DaniTech_2DAnimatorController AnimatorController_Entity;

    [Header("마우스 위치설정")]
    [SerializeField] private GameObject _gameObjectAttackArrow;
    
    [SerializeField] private GameObject _attackPoint;
    [SerializeField] private Camera _camera;

    [Header("스킬 관련")]
    [SerializeField] private string id_SkillObject_Normal;
    [SerializeField] private string id_SkillObject_First;
    [SerializeField] private Transform tranform_SkillObjectRoot;

    [Header("전투 관련 정보")]
    [SerializeField] private int _maxHp;
    [SerializeField] private int _playerHp = 100;
    [SerializeField] private int _playerSp = 100;

    [SerializeField] private int _playerBaseAtk = 100;

    // 우선 직접 들고 있다가 추후에 UI매니저한테 요청하도록 개선해볼 것
    [SerializeField] private DaniTech_ScoreUI _scoreUI;

    private Rigidbody2D _rigidBody;
    private bool _isGrounded;
    private float _horizontalInput;
    private bool _lookRight = true;
    private Vector3 mousePos;
    private bool _isSkillUsing;

    // 추후에는 이런 데이터가 저장될 수 있도록 UI에 있는 것보다 한곳으로 모여지는게 좋다
    private int _currentScore;

    private event Action<int, int> _onHpChanged;
    private event Action<int, int> _onSpChanged;


    void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();

        // 2D 캐릭터가 물리 충돌 시 회전해서 넘어지는 것 방지
        _rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;

        
        _maxHp = _playerHp;
        
    }

    


    private void Start()
    {
        // 나 스스로를 등록한다. -> 씬에 있는 그 2D 플레이어가 등록됨
        DaniTechGameObjectManager.Inst.RegisterLocalPlayer(this);

        //DaniTechUIManager.Instance.AddHudSlot(0, this.gameObject.transform);

        DaniTechUIManager.Instance.AddMainHud(0, this.gameObject.transform);
        InvokeStatChangedEvent();

    }

    void Update()
    {
        // 1. 입력 받기 (Update에서 수행)
        _horizontalInput = Input.GetAxisRaw("Horizontal");

        // 2. 점프 입력
        if (Input.GetButton("Jump") && _isGrounded)
        {
            Jump();
        }

        // 3. 캐릭터 방향 전환 (Flip)
        if (_horizontalInput > 0 && !_lookRight)
        {
            Flip();
        }
        else if (_horizontalInput < 0 && _lookRight) 
        { 
            Flip(); 
        }

        // 이동을 한다라는 판정만 우선 해봅시다
        bool isMoving = (_horizontalInput != 0);
        ChangePlayerState(isMoving ? DaniTech_EntityAnimState.Walk : DaniTech_EntityAnimState.Idle);

        if (Input.GetMouseButtonDown(0))
        {
           
            UseNormalAttack();

            Vector3 lookDirection = _gameObjectAttackArrow.transform.up; // 화살표의 앞방향이 어떤 축인지에 따라 바뀔 수 있음
            if (lookDirection.x < 0 && _lookRight)
            {
                Flip();
            }

            if (lookDirection.x > 0 && !_lookRight)
            {
                Flip();
            }

        }

        if (Input.GetMouseButtonDown(1))
        {
            Vector3 lookDirection = _gameObjectAttackArrow.transform.up; // 화살표의 앞방향이 어떤 축인지에 따라 바뀔 수 있음
            if (lookDirection.x < 0 && _lookRight)
            {
                Flip();
            }

            if (lookDirection.x > 0 && !_lookRight)
            {
                Flip();
            }
            UseFirstSkill();

            
           
        }
        
        FellowMouse();
    }


    private void FellowMouse()
    {
        Vector3 mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);
        float angle = Mathf.Atan2(mousePos.y - _gameObjectAttackArrow.transform.position.y, mousePos.x - _gameObjectAttackArrow.transform.position.x) * Mathf.Rad2Deg;
        _gameObjectAttackArrow.transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
    }
    private void ChangePlayerState(DaniTech_EntityAnimState newState)
    {
        // 이런 곳에 UI나 플레이어의 별도 처리를 넣어줄 수도 있다


        // 우선 애니메이션만 바꿔 봅시다
        AnimatorController_Entity.SetState(newState);
    }

    void FixedUpdate()
    {
        // 4. 지면 체크 (물리 연산 전 수행)
        _isGrounded = Physics2D.OverlapCircle(_groundCheck.position, _checkRadius, _groundLayer);

        // 5. 좌우 이동 처리
        Move();
    }

    void Move()
    {
        // Y축 속도는 유지하면서 X축 속도만 변경 (관성 유지)
        _rigidBody.linearVelocity = new Vector2(_horizontalInput * _moveSpeed, _rigidBody.linearVelocity.y);
    }

    void Jump()
    {
        // 순간적인 힘을 위로 가함
        _rigidBody.linearVelocity = new Vector2(_rigidBody.linearVelocity.x, _jumpForce);
    }

    void Flip()
    {
        _lookRight = !_lookRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }

    // 에디터 뷰에서 지면 체크 범위를 시각적으로 확인
   

    // 6) 적 충돌 시 처리를 해보자
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 6-1) 플레이어의 > 콜리전에 충돌한 객체가 어떤 Tag인지 1차 검사한다.
            // 지면 같은 오브젝트와 점프시 충돌이 계속 오므로 이렇게 태그로 먼저 비교하는게 좋다
            // 중단점을 찍어보면서 확인 추천
        if (collision.gameObject.CompareTag("Enemy") == false)
        {
            return;
        }

        // 6-2) 충돌한 몬스터의 정보를 받아오려고 시도해보자
        var enemyComponent = collision.gameObject.GetComponent<DaniTech_2DEnemy>();
        if (enemyComponent == null)
        {
            Debug.Log($"충돌한 적 객체에서 컴포넌트를 찾을 수 없습니다 : {gameObject.name}");
            return;
        }

        // 6-3) 충돌된 오브젝트를 플레이어가 직접 제거하는게 아니라, Id로 게임오브젝트매니저한테 삭제를 요청한다
        DaniTechGameObjectManager.Inst.RequestDestroyEntityObject(enemyComponent.EntityInstancId);

        // 6-4) 피그미를 잡으면 스코어를 올려주자!
        AddGameScore();
    }

    private void AddGameScore()
    {
        // 7) 여기서 맥락 -> UI를 갱신해주기 위해 과연 플레이어가 이렇게 UI를 직접
            // 알고 있는게 좋은걸까?

        _currentScore++;
        _scoreUI.AddGameScore(_currentScore);
    }



    private bool CheckSkillUseble(bool isShowMsg = true)
    {
        if (_isSkillUsing == true)
        {
            if (isShowMsg == true)
            {
                Debug.Log("사용중");
            }
            return false;
        }

        return true;
        
    }

    
    public void UseNormalAttack()
    {
        if (CheckSkillUseble(isShowMsg:false)==false) { return; }
        ChangePlayerState(DaniTech_EntityAnimState.Atk);

        CreateProjectSkillObject(id_SkillObject_Normal);

        StartCoroutine(CoStartNormalAttack());
    }

    public void UseFirstSkill()
    {
        if (CheckSkillUseble(isShowMsg: false) == false) { return; }
        ChangePlayerState(DaniTech_EntityAnimState.Atk);

        CreateProjectSkillObject(id_SkillObject_First);

        StartCoroutine(CoStartFirstSkill());
    }
    public void UseSecondSkill()
    {

    }
    public void UseThirdSkill()
    {
        CreateProjectSkillObject(id_SkillObject_Normal);
    }
   

    private void CreateProjectSkillObject(string id_SkillObject)
    {

        var tag = this.gameObject.tag;

        // 람다식을 이용해 생성 완료 후 gObj와 "똑같은 작용"을 하도록 로직을 넘깁니다.
        DaniTechGameObjectManager.Inst.CreateSKillObject(id_SkillObject, tranform_SkillObjectRoot, (createdObj) =>
        {
            if (createdObj == null) return;

            // gObj에서 하던 컴포넌트 세팅 및 초기화를 그대로 수행
            var skillProjectileComponent = createdObj.GetComponent<HJD_SkillBase>();
            if (skillProjectileComponent == null) return;

            skillProjectileComponent.InitSkillObject(0, tranform_SkillObjectRoot.position, _playerBaseAtk, tag, OnMonsterCollied);
        }).Forget();

        
    }

    private void OnMonsterCollied(int monsterInstanceId, int skillDamage)
    {
        // 게임 오브젝트 매니저는 모든 몬스터를 관리한다
        // 그 자료구조는 Dictionary로 key - instanceId다
        // 몬스터를 게임오브젝트 매니저를 통해 받아올 수 있다!


        // 몬스터때도 구현했던 2번 방식) 플레이어한테 스킬이 충돌정보를 알려주기만 하고, 실제 몬스터와의 상호작용은 플레이어가
        // 주도권을 갖고 한다!
        var monsterComponent = DaniTechGameObjectManager.Inst.GetMonsterObjectByInstanceId(monsterInstanceId);
        if (monsterComponent == null) return;

        Debug.LogWarning($"플레이어가 {monsterInstanceId}에 데미지 {skillDamage} 부여");
        monsterComponent.TakeDamage(skillDamage);
    }

    IEnumerator CoStartNormalAttack()
    {
        yield return new WaitForSeconds(1.0f);
        //Prefab_SkillObject.gameObject.SetActive(false);

    }

    IEnumerator CoStartFirstSkill()
    {
        yield return new WaitForSeconds(1.0f);
        //Prefab_SkillObject.gameObject.SetActive(false);

    }


    public void TakeDamage(int damage)
    {
        _playerHp -= damage;
        Debug.Log($"{_playerHp}");

        InvokeStatChangedEvent();
        if (_playerHp < 0)
        {
            // 죽음 처리를 여기서 해두고
            PlayerDie();
        }
    }

    public void PlayerDie()
    {
        // bool _isAlive = false;
        DaniTechGameManager.Inst.RespawnPlayer();
        _playerHp = 0;
        _playerHp += _maxHp;
        InvokeStatChangedEvent();

        // Destroy(this.gameObject);
        //DaniTechUIManager.Instance.RemoveHudSlot(0);
    }

    public void BindOnstatChangedEvent(Action<int,int> hpChangeCallback, Action<int, int> spChangeCallback)
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
        _onHpChanged?.Invoke(_playerHp,_maxHp);
       // _onSpChanged?.Invoke(_playerSp);
    }


    private void OnDrawGizmos()
    {
        if (_groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_groundCheck.position, _checkRadius);
        }
    }

    public void AddHp(int hp)
    {
        _playerHp += hp;
        InvokeStatChangedEvent();
    }

    public void AddAtk(int atk)
    {
        _playerBaseAtk += atk;
    }
}
