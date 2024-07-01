using System;
using Microsoft.AspNetCore.SignalR.Client;
using TMPro;
using UnityEngine;

namespace FirstRoom
{
    public class FirstRoom : MonoBehaviour
    {
        [SerializeField] private TMP_Text remainTimeText;

        private HubConnection _connection;
        private const string ServerAddress = "https://escaperoom.ddnsking.com";

        void Start()
        {
            Debug.Log($"MH1: Counter: {StaticData.RemainTime}");
        }

        void Update()
        {
            try
            {
                TimeCounter.Instance.UpdateTime();
                UpdateTimeText();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        void UpdateTimeText()
        {
            try
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
            catch (NullReferenceException e)
            {
                Console.WriteLine(e);
            }
        }
    }
}