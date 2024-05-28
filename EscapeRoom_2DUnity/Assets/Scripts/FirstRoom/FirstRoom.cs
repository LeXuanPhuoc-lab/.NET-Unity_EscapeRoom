using TMPro;
using UnityEngine;

namespace FirstRoom
{
    public class FirstRoom : MonoBehaviour
    {
        [SerializeField] private TMP_Text remainTimeText;

        void Start()
        {
            Debug.Log($"MH1: Counter: {StaticData.RemainTime}");
        }

        void Update()
        {
            TimeCounter.Instance.UpdateTime();
            UpdateTimeText();
        }

        void UpdateTimeText()
        {
            remainTimeText.color = StaticData.RemainTime switch
            {
                < 60 => Color.red,
                < 180 => Color.yellow,
                _ => Color.green
            };

            float minutes = Mathf.FloorToInt(StaticData.RemainTime / 60);
            float seconds = Mathf.FloorToInt(StaticData.RemainTime % 60);

            remainTimeText.text = $"{minutes:00}:{seconds:00}";
        }
    }
}