using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Home
{
    public class CreateRoomForm : MonoBehaviour
    {
        [SerializeField] private TMP_InputField roomName;
        [SerializeField] private TMP_InputField totalPlayer;
        [SerializeField] private TMP_InputField endTimeToMinute;

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

            Debug.Log(1);

            HomeManager.Instance.CreateRoom(new CreateRoomBody()
            {
                Username = "",
                TotalPlayer = int.Parse(totalPlayerValue),
                RoomName = roomNameValue,
                EndTimeToMinute = int.Parse(endTimeToMinuteValue)
            });


            // var room = new Room
            // {
            //     RoomName = roomNameValue,
            //     TotalPlayer = int.Parse(totalPlayerValue),
            //     EndTimeToMinute = int.Parse(endTimeToMinuteValue)
            // };
            //
            // GameManager.Instance.CreateRoom(room);
            //
            // gameObject.SetActive(false);
        }
    }
}