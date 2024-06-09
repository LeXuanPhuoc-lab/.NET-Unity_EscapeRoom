using System;
using TMPro;
using UnityEngine;

namespace Home
{
    public class FindRoomMenu : MonoBehaviour
    {
        [SerializeField] private TMP_InputField roomCode;
        // [Serializable] private TMP_

        private void OnEnable()
        {
            GenerateRoomLists();
        }

        private async void GenerateRoomLists()
        {
            
        }

        public void HandleFindRandomRoom()
        {
            HomeManager.Instance.FindRandomRoom();
        }

        public void HandleJoinRoom()
        {
            var roomCodeValue = roomCode.text;

            if (string.IsNullOrEmpty(roomCodeValue))
            {
                return;
            }

            HomeManager.Instance.JoinRoom(roomCodeValue);
        }
    }
}