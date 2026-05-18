using UnityEngine;
using UnityEngine.UI;

public class HJD_ScoreUI : MonoBehaviour
{
    [SerializeField] private Text Text_Score;

    public void AddGameScore(int currentScore)
    {
        Text_Score.text = $"µ¿Àü ¼ö:{currentScore}";


       
    }

}
