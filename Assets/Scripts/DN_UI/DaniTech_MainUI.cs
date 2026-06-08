using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class DaniTech_MainUI : DaniTechUIBase
{
    [SerializeField] private DaniTechUIButton Btn_OpenInventory;
    [SerializeField] private DaniTechUIButton Btn_GameBook;

    [Header("스킬 버튼")]
    [SerializeField] private DaniTechUIButton Btn_UseNormalAttack;
    

    [Header("스킬 버튼")]
    [SerializeField] private Slider Slider_Hp;
    [SerializeField] private Slider Slider_Sp;
    [SerializeField] private TextMeshProUGUI _textMesh_Hp;

    private int _instanceId;
    private Transform _targetTransform;

    private void OnEnable()
    {
        //Btn_StartBattle.BindOnClickButtonEvent(OnClick_StartBattle);
        //Btn_MonsterSpawn.BindOnClickButtonEvent(OnClicK_MonsterSpawn);
        //Btn_SaveGame.BindOnClickButtonEvent(OnClick_SaveGame);
        //Btn_MyProfile.BindOnClickButtonEvent(OnClick_OpenMyProfile);

        Btn_OpenInventory.BindOnClickButtonEvent(OnClick_OpenInventory);
        Btn_GameBook.BindOnClickButtonEvent(OnClick_OpenGameBook);
        Btn_UseNormalAttack.BindOnClickButtonEvent(OnClick_UseNormalAttack);
        
    }

    private void OnClick_UseNormalAttack()
    {
        //DaniTechGameManager.Inst.LocalPlayer.UseNormalAttack();

        var localPlayer = DaniTechGameManager.Inst.GetLocalPlayer(); //  DaniTechGameObjectManager.Inst.GetLocalPlayer();
        localPlayer.UseNormalAttack();
    }

    private void OnClick_UseFirstSkill()
    {
        var localPlayer = DaniTechGameManager.Inst.GetLocalPlayer(); //  DaniTechGameObjectManager.Inst.GetLocalPlayer();
        localPlayer.UseFirstSkill();
    }

    private void OnClick_UseSecondSkill()
    {
        var localPlayer = DaniTechGameManager.Inst.GetLocalPlayer(); //  DaniTechGameObjectManager.Inst.GetLocalPlayer();
        localPlayer.UseSecondSkill();

    }
    private void OnClick_UseThirdSkill()
    {
        var localPlayer = DaniTechGameManager.Inst.GetLocalPlayer(); //  DaniTechGameObjectManager.Inst.GetLocalPlayer();
        localPlayer.UseThirdSkill();

    }


    public void OnClick_OpenGameBook()
    {
        Debug.Log("됨");

        DaniTechUIManager.Instance.OpenContentUI(DaniTechUIType.DNGameBookUI);
    }

    public void OnClick_OpenInventory()
    {
        DaniTechUIManager.Instance.OpenInventoryPopup();
        DaniTechGameManager.Inst.SaveData();
    }

    public void OnClick_OpenMyProfile()
    {
        //UIManager.Instance.OpenMyProfilePopup("character_hellena_01");
        DaniTechUIManager.Instance.OpenInventoryPopup();
        Debug.LogWarning("프로필 오픈");
    }

    public void OnClick_StartBattle()
    {
        DaniTechUIManager.Instance.OpenSimplePopup("배틀 스타트!");
        Debug.LogWarning("배틀 스타트");
    }

    public void OnClicK_MonsterSpawn()
    {
        Debug.LogWarning("몬스터 스폰");
    }

    public void OnClick_SaveGame()
    {
        DaniTechGameManager.Inst.SaveData();
        DaniTechUIManager.Instance.OpenSimplePopup("게임 세이브");
        Debug.LogWarning("게임 세이브");
    }


    public void InitMainHud(int instanceId, Transform targetTranform)
    {
        _instanceId = instanceId;
        _targetTransform = targetTranform;


        TryBindStatChangedEvent(targetTranform.gameObject);
    }

    private void TryBindStatChangedEvent(GameObject gObj)
    {
        var player = gObj.GetComponent<DaniTech_2DPlayer>();
        if (player != null)
        {
            player.BindOnstatChangedEvent(OnTargetEntityHpChange, OnTargetEntitySpChange);
            return;
        }
        var monster = gObj.GetComponent<DaniTech_GameMonster_Dog>();
        if (monster != null)
        {
            monster.BindOnstatChangedEvent(OnTargetEntityHpChange, OnTargetEntitySpChange);
            return;
        }


    }
    private void OnTargetEntityHpChange(int curHp, int maxHp)
    {
        Slider_Hp.value = (curHp / (float)maxHp);
        _textMesh_Hp.text = curHp.ToString();

    }

    private void OnTargetEntitySpChange(int curSp, int maxSp)
    {
        Slider_Sp.value = (curSp / (float)maxSp);


    }
}
