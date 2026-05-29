using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections;


public class HJD_SkillBase : MonoBehaviour
{
    [Header("공통 속성")]
    protected int _damage;
    protected int _ownerInstanceId;
    [SerializeField] protected float skillLifeTime = 3.0f;


    [SerializeField] protected String SpritePath_Skill;

    [SerializeField] protected SpriteRenderer Sprite_Skill;

    [SerializeField] protected float _moveSpeed = 5f;


    protected event Action<int, int> _onSkillCollision;

    private void Awake()
    {
        LoadAndSetSkillSprite(SpritePath_Skill);
    }

    private void OnEnable()
    {
    StartCoroutine(ExecuteAfterTime(skillLifeTime));

    }

    private void OnDisable()
    {
        _onSkillCollision = null;
    }

    public void InitSkillObject(int ownerInstanceId, Vector3 playerPos, int damage, string parentTag, Action<int, int> onSkillCollision = null)
    {
        this.transform.position = playerPos;

        _damage = damage;
        _ownerInstanceId = ownerInstanceId;

        _onSkillCollision = onSkillCollision;
        // 소환자의 Tag정보를 기입한다
        this.gameObject.tag = parentTag;
    }


   
   

    private void LoadAndSetSkillSprite(string spriteDataId)
    {



        var skillspriteData = DaniTechGameDataManager.Instance.GetSkill(spriteDataId);
        if (skillspriteData == null)
        {
            Debug.LogWarning($"skillsprite 데이터를 불러올 수 없습니다! 경로:{spriteDataId}");
            return;
        }

        string skillspritePath = skillspriteData.SpritePath;
        if (string.IsNullOrEmpty(skillspritePath) == true)
        {
            Debug.LogWarning($"skillsprite 데이터에 아이콘 경로가 존재하지 않습니다.");
            return;
        }

        DaniTechGameUtil.LoadAndSetSprite(Sprite_Skill, skillspritePath).Forget();

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        CheckCollision(collision);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        CheckCollision(collision.collider);
    }

    private void CheckCollision(Collider2D collision)
    {
        // Owner가 0번이면 무조건 플레이어다
        bool isOwnerPlayer = (_ownerInstanceId == 0);

        // 투사체가 충돌한 오브젝트의 Tag가 플레이어라면?
        if (collision.CompareTag("Player") && (isOwnerPlayer == false))
        {
            // 플레이어라면 직접 플레이어에게 투사체가 데미지를 부여해봅시다
            var player = DaniTechGameObjectManager.Inst.GetLocalPlayer();
            player.TakeDamage(_damage);

            // 스킬은 오브젝트 매니저를 통해서 만들어지지는 않았으므로 직접 스스로 제거해봅시다
            // 몬스터 -> 오브젝트 매니저를 통해서 제거 (UI매니저와 동일한 프로세스)
            // 스킬은 직접 스스로 제거
            SKillLife();
        }
        else if (collision.CompareTag("Enemy") && (isOwnerPlayer))  // 몬스터다! (대신 Tag 오탈자, 대소문자 꼭 확인)
        {
            var gObj = collision.gameObject; // 슬라임 gameObject
            if (gObj == null) return;

            var monsterComponent = gObj.GetComponent<DaniTech_GameMonster_Dog>();
            if (monsterComponent == null) return;

            // 1번 방식 - 투사체가 직접 몬스터에게 데미지를 입힌다 : 다만, 게임오브젝트매니저를 통하는게 조금 더 최종폼
            // monsterComponent.TakeDamage(skillDamage);

            int instId = monsterComponent.GetMonsterInstanceId();
            _onSkillCollision?.Invoke(instId, _damage);

            SKillLife();
            
        }
    }

    protected virtual void SKillLife()
    {

    }


    IEnumerator ExecuteAfterTime(float time)
    {
        Debug.Log("3초 대기를 시작합니다.");

        // 지정한 초(time)만큼 유니티가 대기합니다.
        yield return new WaitForSeconds(time);

        Destroy(this.gameObject);


    }
}
