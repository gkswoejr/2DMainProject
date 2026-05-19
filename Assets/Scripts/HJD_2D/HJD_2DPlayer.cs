using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class HJD_2DPlayer : MonoBehaviour
{
    [Header("이동 설정")]
    [SerializeField] private float _moveSpeed = 8f;
    [SerializeField] private float _jumpForce = 12f;

    [Header("지면 체크 설정")]
    [SerializeField] private Transform _groundCheck;    
    [SerializeField] private float _checkRadius = 0.5f; 
    [SerializeField] private LayerMask _groundLayer;   
    [Header("애니메이션 설정")]
    [SerializeField] private HJD_AnimatorCotroller animatorCotroller;

    [SerializeField] private HJD_ScoreUI _scoreUI;

    [Header("마우스 위치설정")]
    [SerializeField] private GameObject _attackArrow;
    [SerializeField] private GameObject _attackPoint;
    [SerializeField] private Camera _camera;

    private Rigidbody2D _rigidBody;
    private bool _isGrounded;
    private float _horizontalInput;
    private bool _lookRight = true;

    private int _currentScore;

    

    void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();

        // 2D 캐릭터가 물리 충돌 시 회전해서 넘어지는 것 방지
        _rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;

        _camera = Camera.main;
    }

    void Update()
    {
        // 1. 입력 받기 (Update에서 수행)
        _horizontalInput = Input.GetAxisRaw("Horizontal");

        // 2. 점프 입력
        if (Input.GetButtonDown("Jump") && _isGrounded)
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

        bool IsWalk = (_horizontalInput != 0);
        Debug.Log(_horizontalInput);
        ChangePlayerState(IsWalk ? EntiyAnimState.Walk : EntiyAnimState.Idle);

        if (Input.GetKeyDown(KeyCode.F))
        {
            ChangePlayerState(EntiyAnimState.Attack);
            HJD_BattleManager.Instance.Attack(_attackPoint.transform);
        }

        FellowMouse();

    }
    private void FellowMouse()
    {
        Vector3 mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);
        float angle = Mathf.Atan2(mousePos.y - _attackArrow.transform.position.y, mousePos.x - _attackArrow.transform.position.x) * Mathf.Rad2Deg;
        _attackArrow.transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
    }

    private void ChangePlayerState(EntiyAnimState newstate)
    {
        animatorCotroller.SetState(newstate);

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
        _rigidBody.linearVelocity = new Vector2(_horizontalInput * _moveSpeed, _rigidBody.linearVelocity.y);
    }

    void Jump()
    {
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
    private void OnDrawGizmos()
    {
        if (_groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_groundCheck.position, _checkRadius);
        }
    }

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

        

        collision.gameObject.SetActive(false);

        
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
}
