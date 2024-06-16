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
        [SerializeField] public RectTransform scrollView;

        private void OnEnable()
        {
            GenerateRoomLists();
        }

        private async void GenerateRoomLists()
        {
            foreach (Transform child in scrollView)
            {
                if (!child.gameObject != roomItemPrefab)
                    Destroy(child.gameObject);
            }

            var roomList = await HomeManager.Instance.GetRooms();

            foreach (var room in roomList)
            {
                AddRoomItem(room);
            }
        }

        private void AddRoomItem(GameSessionDto room)
        {
            var newRoomItem = Instantiate(roomItemPrefab, scrollView);
            newRoomItem.GetComponentInChildren<TMP_Text>().text = room.SessionName;
            newRoomItem.GetComponent<Button>().onClick.AddListener(() => HandleJoinRoomBySelect(room.SessionId));
            newRoomItem.GetComponent<Button>().gameObject.SetActive(true);
            var childCount = scrollView.childCount;
            newRoomItem.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -100f * childCount + 92);
            scrollView.sizeDelta = new Vector2(scrollView.sizeDelta.x, 100 * childCount);
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