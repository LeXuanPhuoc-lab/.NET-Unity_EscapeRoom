using Home;
using Inputs;
using Microsoft.AspNetCore.SignalR.Client;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FirstRoom
{
    public class FirstRoom : MonoBehaviour
    {
        [SerializeField] private TMP_Text remainTimeText;
        [SerializeField] private GameObject menuInGame;

        private HubConnection _connection;
        private const string ServerAddress = "https://localhost:7000";

        void Start()
        {
            menuInGame.SetActive(false);
            Debug.Log($"MH1: Counter: {StaticData.RemainTime}");
        }

        void Update()
        {
            TimeCounter.Instance.UpdateTime();
            UpdateTimeText();
            if (UserInput.Instance.Controls.Escape.Escape.WasPressedThisFrame())
            {
                Debug.Log($"press escape");
                menuInGame.SetActive(!menuInGame.activeSelf);
            }
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

        public void HandleContinue()
        {
            menuInGame.SetActive(false);
        }

        public async void HandleOutRoom()
        {
            var success = await APIManager.Instance.OutRoomAsync();
            if (success)
            {
                Debug.Log("Out Successfully");
                SceneManager.LoadScene("Home");
            }
        }
    }
}