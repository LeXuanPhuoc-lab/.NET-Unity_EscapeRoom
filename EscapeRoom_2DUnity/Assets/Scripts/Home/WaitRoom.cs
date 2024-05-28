using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Home
{
    public class WaitRoom : MonoBehaviour
    {
        [SerializeField] private TMP_Text roomText;
        [SerializeField] private TMP_Text totalPlayerText;
        [SerializeField] private TMP_Text durationText;
        [SerializeField] private GameObject startGameButton;
        [SerializeField] private Button readyButton;
        [SerializeField] private TMP_Text readyButtonText;
        // [SerializeField] private TMP_Text outRoomButton;

        private bool _isHost;

        public void UpdateStates()
        {
            Debug.Log("UpdateStates");
            roomText.text = $"Room: {HomeManager.Instance.gameSession.SessionName}";
            totalPlayerText.text =
                $"Total Player: {HomeManager.Instance.gameSession.PlayerGameSessions.Count()}/{HomeManager.Instance.gameSession.TotalPlayer}";
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

        public void HandleReadySuccess()
        {
            readyButton.interactable = false;
            readyButtonText.text = "You are ready";
        }

        public void HandleOutRoom()
        {
            HomeManager.Instance.OutRoom();
        }
    }
}