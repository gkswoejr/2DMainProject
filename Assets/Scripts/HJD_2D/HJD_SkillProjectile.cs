using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class HJD_SkillProjectile : MonoBehaviour
{

    [SerializeField] SpriteRenderer Sprite_Skill;
    [SerializeField] private float _moveSpeed = 5f;

    //[SerializeField] private Vector3 _moveDirection = new Vector3(0, 1, 0);


    /*public void InitSkillObject(bool isDirRight)
    {
        _moveDirection = isDirRight ? new Vector3(0,1,0) : new Vector3(0, -1,0);
    }
*/
   
    private void Awake()
    {
        LoadAndSetSkillSprite("skill_Tree_01");
    }
    void Update()
    {
        transform.Translate(Vector3.up * _moveSpeed * Time.deltaTime);
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
}
