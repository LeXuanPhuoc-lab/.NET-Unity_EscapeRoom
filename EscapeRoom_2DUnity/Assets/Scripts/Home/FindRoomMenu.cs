using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Home
{
    public class FindRoomMenu : MonoBehaviour
    {
        [SerializeField] private TMP_InputField roomCodeInput;
        [SerializeField] public GameObject roomItemPrefab;
        public Transform scrollView;

        private void OnEnable()
        {
            GenerateRoomLists();
        }

        private async void GenerateRoomLists()
        {
            var roomList = await HomeManager.Instance.GetRooms();

            foreach (var room in roomList)
            {
                var newRoomItem = Instantiate(roomItemPrefab, scrollView);
                newRoomItem.GetComponentInChildren<TMP_Text>().text = room.SessionName;
                newRoomItem.GetComponent<Button>().onClick.AddListener(() => HandleJoinRoomBySelect(room.SessionId));
            }
        }

        private void HandleJoinRoomBySelect(int sessionId)
        {
            HomeManager.Instance.JoinRoomBySelect(sessionId);
        }

        public void HandleFindRandomRoom()
        {
            HomeManager.Instance.FindRandomRoom();
        }

        public void HandleJoinRoom()
        {
            var roomCodeInputValue = roomCodeInput.text;

            if (string.IsNullOrEmpty(roomCodeInputValue))
            {
                return;
            }

            HomeManager.Instance.JoinRoomByCode(roomCodeInputValue);
        }
    }
}