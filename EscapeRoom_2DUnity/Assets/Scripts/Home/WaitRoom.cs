using System;
using System.Linq;
using Microsoft.AspNetCore.SignalR.Client;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Home
{
    public class WaitRoom : MonoBehaviour
    {
        [SerializeField] private TMP_Text roomText;
        [SerializeField] private TMP_Text totalPlayerText;
        [SerializeField] private TMP_Text totalReadyText;
        [SerializeField] private TMP_Text durationText;
        [SerializeField] private GameObject startGameButton;
        [SerializeField] private Button readyButton;
        [SerializeField] private TMP_Text readyButtonText;
        // [SerializeField] private TMP_Text outRoomButton;

        private bool _isHost;

        private void Awake() {
            Debug.Log("Awake in wait room");
        }
        
        private void OnEnable() {
            HomeManager.Instance.ConnnectSignalRServer();    
        }

        public void UpdateStates()
        {
            Debug.Log("UpdateStates");
            roomText.text = $"Room: {HomeManager.Instance.gameSession.SessionName}";
            totalPlayerText.text =
                $"Total Player: {HomeManager.Instance.gameSession.PlayerGameSessions.Count()}/{HomeManager.Instance.gameSession.TotalPlayer}";
            totalReadyText.text = $"Total Ready: 0/{HomeManager.Instance.gameSession.TotalPlayer}";
            durationText.text = $"Duration: {HomeManager.Instance.gameSession.EndTime.ToString()}";
            Debug.Log("UpdateStates2");
            var hostPlayer = HomeManager.Instance.gameSession.PlayerGameSessions.FirstOrDefault(ps => ps.IsHost);
            Debug.Log("UpdateStates3");
            Debug.Log("hostPlayer");
            Debug.Log(hostPlayer);
            _isHost = hostPlayer is not null && hostPlayer.Player.Username == StaticData.Username;
            Debug.Log("UpdateStates4");

            if (!_isHost)
            {
                startGameButton.SetActive(false);
            }
        }

        public void ProcessFindOrReadyUpdate(int totalPlayerInSession, int sessionPlayerCap, int totalReadyPlayers)
        {
            totalPlayerText.text =
                $"Total Player: {totalPlayerInSession}/{sessionPlayerCap}";
            totalReadyText.text = $"Total Ready: {totalReadyPlayers}/{HomeManager.Instance.gameSession.TotalPlayer}";
        }

        public void ResetReadyButton()
        {
            Debug.Log("ResetReadyButton");
            readyButton.interactable = true;
            readyButtonText.text = "Ready";
        }

        public void HandleReady()
        {
            HomeManager.Instance.Ready();
        }

        public void SetButtonTextColor(string hexColor)
        {
            if (UnityEngine.ColorUtility.TryParseHtmlString(hexColor, out Color newColor))
                {
                    readyButton.image.color = newColor;
                }
                else
                {
                    Debug.LogError("Invalid hex color code");
                }
        }
        
        public void HandleReadySuccess()
        {
            // readyButton.interactable = false;
            if(readyButtonText.text == "You are ready"){
                readyButtonText.text = "Ready";
                SetButtonTextColor("#FFF200");
            }else{
                readyButtonText.text = "You are ready";
                SetButtonTextColor("#08F300");
            }
        }

        public void HandleOutRoom()
        {
            HomeManager.Instance.OutRoom();
        }

        public void HandleStartRoom()
        {
            HomeManager.Instance.StartRoom();
        }
    }
}