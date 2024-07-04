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

        private const float itemHeight = 100f; // Height of each item
        private const float itemSpacing = 10f; // Spacing between items

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

            if (roomList != null && roomList.Count > 0)
            {
                foreach (var room in roomList)
                {
                    AddRoomItem(room);
                }
            }
        }

        private void AddRoomItem(GameSessionDto room)
        {
            var newRoomItem = Instantiate(roomItemPrefab);
            newRoomItem.transform.SetParent(scrollView.transform, false);

            var textComponent = newRoomItem.GetComponentInChildren<TMP_Text>();
            textComponent.text = $"Room: {room.SessionName}";;
            textComponent.alignment = TMPro.TextAlignmentOptions.Left;
            // newRoomItem.GetComponentInChildren<TMP_Text>().text = $"Room Name: {room.SessionName}";
            newRoomItem.GetComponent<Button>().onClick.AddListener(() => HandleJoinRoomBySelect(room.SessionId));
            newRoomItem.GetComponent<Button>().gameObject.SetActive(true);

            // var childCount = scrollView.childCount;
            // // newRoomItem.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -100f * childCount + 496);
            // // newRoomItem.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -100f * (childCount - 1));
            // // scrollView.sizeDelta = new Vector2(scrollView.sizeDelta.x, 100 * childCount);

            // // Calculate the total height including the spacing between items
            // var totalHeight = (itemHeight + itemSpacing) * childCount - itemSpacing;

            // // Update the size of the scroll view content
            // scrollView.sizeDelta = new Vector2(scrollView.sizeDelta.x, totalHeight);

            // // Calculate the anchored position for the new item
            // var rectTransform = newRoomItem.GetComponent<RectTransform>();
            // rectTransform.anchoredPosition = new Vector2(0, -(itemHeight + itemSpacing) * (childCount - 1));
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