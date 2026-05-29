using UnityEngine;
using UnityEngine.UI;

public class EndingUI : DaniTechUIBase
    
{
    [SerializeField] Button Button;
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    private void Awake()
    {
        Button.onClick.AddListener(OnClickExit);
    }

    private void OnClickExit() 
    {
        Application.Quit();
    }
}
