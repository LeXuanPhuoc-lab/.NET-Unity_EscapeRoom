using ScoreBoard;
using TMPro;
using UnityEngine;

public class ScoreBoardEntryUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI rankText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI scoreText;

    public void Initialise(ScoreBoardEntryData entryData, int rank)
    {
        rankText.text = rank.ToString();
        nameText.text = entryData.entryName;
        scoreText.text = entryData.entryScore.ToString();
    }
}