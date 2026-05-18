using UnityEngine;

public class RobbyUI : DaniTechUIBase
{
    [SerializeField] private DaniTechUIButton Button_Start;
    [SerializeField] private DaniTechUIButton Button_Exit;



    private void OnEnable()
    {
        Button_Start.BindOnClickButtonEvent(OnClick_GameStartButton);
        Button_Exit.BindOnClickButtonEvent(OnClick_GameExitButton);
    }
    private void OnClick_GameStartButton()
    {
        Debug.Log("게임시작");
        DaniTechUIManager.Instance.CloseContentUI(DaniTechUIType.DNRobbyUI);
    }

    private void OnClick_GameExitButton()
    {
        Debug.Log("게임종료");

        DaniTechGameManager.Inst.SaveAndEndGame();

    }

   



   
}
