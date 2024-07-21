using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;

namespace Home
{
    public class FindRoomMenu : MonoBehaviour
    {
        [SerializeField] private TMP_InputField roomCodeInput;
        [SerializeField] public GameObject roomItemPrefab;
        [SerializeField] public RectTransform scrollView;

        private const float itemHeight = 100f; // Height of each item
        private const float itemSpacing = 10f; // Spacing between items

        private HubConnection hubConnection;

        private async Task OnEnable()
        {
            await HomeManager.Instance.ConnectSignalRServer();
            InitializeHubConnection();
            SubscribeToConnectionEvents();
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

        private void InitializeHubConnection()
        {
            hubConnection = StaticData.HubConnection;

            if (hubConnection == null)
            {
                Debug.LogError("HubConnection is not initialized.");
                return;
            }

            SubscribeToUpdateRoomList();

            if (hubConnection.State == HubConnectionState.Connected)
            {
                Debug.Log("Connected and listening from server.");
            }
            else
            {
                Debug.LogError("HubConnection is not connected. Current state: " + hubConnection.State);
            }
        }

        private void SubscribeToConnectionEvents()
        {
            hubConnection.Reconnecting += error =>
            {
                Debug.LogWarning("Reconnecting...");
                return Task.CompletedTask;
            };

            hubConnection.Reconnected += connectionId =>
            {
                Debug.Log("Reconnected. Re-subscribing to events...");
                SubscribeToUpdateRoomList();
                return Task.CompletedTask;
            };

            hubConnection.Closed += async error =>
            {
                Debug.LogError("Connection closed. Attempting to reconnect...");
                await hubConnection.StartAsync();
            };
        }

        private void SubscribeToUpdateRoomList()
        {
            Debug.Log("Subscribing to Update room list event...");

            hubConnection.On<string>("OnTriggerCreateRoom", (roomList) =>
            {
                Debug.Log("Receive list room from server successfully, with data: " + roomList);
                var roomListConverted = JsonConvert.DeserializeObject<List<GameSessionDto>>(roomList);

                Debug.Log(scrollView.childCount);
                foreach (Transform child in scrollView)
                {
                    if (!child.gameObject != roomItemPrefab)
                        Destroy(child.gameObject);
                }

                foreach (var room in roomListConverted)
                {
                    AddRoomItem(room);
                }
            });
        }

        private void AddRoomItem(GameSessionDto room)
        {
            string sessionName = room.SessionName;
            if (sessionName.Length > 15)
            {
                sessionName = sessionName.Substring(0, 12) + "...";
            }
            var newRoomItem = Instantiate(roomItemPrefab);
            newRoomItem.transform.SetParent(scrollView.transform, false);

            var textComponent = newRoomItem.GetComponentInChildren<TMP_Text>();
            textComponent.fontSize = 33;
            textComponent.text = $"Room name: {sessionName}\nTotal players: {room.PlayerGameSessions.ToList().Count}/{room.TotalPlayer}\n"; 
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