using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Home
{
    public class CreateRoomForm : MonoBehaviour
    {
        [SerializeField] private TMP_InputField roomName;
        [SerializeField] private TMP_InputField totalPlayer;
        [SerializeField] private TMP_InputField endTimeToMinute;
        [SerializeField] private Toggle isPrivate;

        public void ResetInputs()
        {
            roomName.text = string.Empty;
            totalPlayer.text = string.Empty;
            endTimeToMinute.text = string.Empty;
        }

        private void OnEnable()
        {
            ResetInputs();
        }

        public void HandleCreateRoom()
        {
            var roomNameValue = roomName.text;
            var totalPlayerValue = totalPlayer.text;
            var endTimeToMinuteValue = endTimeToMinute.text;

            if (string.IsNullOrEmpty(roomNameValue) || string.IsNullOrEmpty(totalPlayerValue) ||
                string.IsNullOrEmpty(endTimeToMinuteValue))
            {
                return;
            }

            Debug.Log(100);

            HomeManager.Instance.CreateRoom(new CreateRoomBody()
            {
                Username = StaticData.Username,
                TotalPlayer = int.Parse(totalPlayerValue),
                RoomName = roomNameValue,
                EndTimeToMinute = int.Parse(endTimeToMinuteValue),
                IsPublic = !isPrivate.isOn
            });
            StaticData.TotalPlayer = int.Parse(totalPlayer.text);
        }
    }
}