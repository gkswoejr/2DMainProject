using UnityEditor.Overlays;
using UnityEngine;

public class DaniTech_MainUI : DaniTechUIBase
{
    [SerializeField] private DaniTechUIButton Btn_MyProfile;
    [SerializeField] private DaniTechUIButton Btn_StartBattle;
    [SerializeField] private DaniTechUIButton Btn_MonsterSpawn;
    [SerializeField] private DaniTechUIButton Btn_OpenInventory;
    [SerializeField] private DaniTechUIButton Btn_SaveGame;
    [SerializeField] private DaniTechUIButton Btn_GameBook;

    [Header("스킬 버튼")]
    [SerializeField] private DaniTechUIButton Btn_UseNormalAttack;
    [SerializeField] private DaniTechUIButton Btn_UseFirstSkill;
    [SerializeField] private DaniTechUIButton Btn_UseSecondSkill;
    [SerializeField] private DaniTechUIButton Btn_UseThirdSkill;




    private void OnEnable()
    {
        Btn_MyProfile.BindOnClickButtonEvent(OnClick_OpenMyProfile);
        Btn_StartBattle.BindOnClickButtonEvent(OnClick_StartBattle);
        Btn_MonsterSpawn.BindOnClickButtonEvent(OnClicK_MonsterSpawn);
        Btn_OpenInventory.BindOnClickButtonEvent(OnClick_OpenInventory);
        Btn_SaveGame.BindOnClickButtonEvent(OnClick_SaveGame);
        Btn_GameBook.BindOnClickButtonEvent(OnClick_OpenGameBook);

        Btn_UseNormalAttack.BindOnClickButtonEvent(OnClick_UseNormalAttack);
        Btn_UseFirstSkill.BindOnClickButtonEvent(OnClick_UseFirstSkill);
        Btn_UseSecondSkill.BindOnClickButtonEvent (OnClick_UseSecondSkill);
        Btn_UseThirdSkill.BindOnClickButtonEvent(OnClick_UseThirdSkill);
    }

    private void OnClick_UseNormalAttack()
    {
        DaniTechGameManager.Inst.LocalPlayer.UseNormalAttack();
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

}
