using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ScoreBoard
{
    public class LoadLeaderBoardSignalR : MonoBehaviour
    {
        public HubConnection hubConnection;

        private void Start()
        {
            InitializeHubConnection();
            SubscribeToConnectionEvents();
        }

        private void InitializeHubConnection()
        {
            hubConnection = StaticData.HubConnection;

            if (hubConnection == null)
            {
                Debug.LogError("HubConnection is not initialized.");
                return;
            }

            SubscribeToShowLeaderBoardEvent();

            if (hubConnection.State == HubConnectionState.Connected)
            {
                Debug.Log("Connected and listening to show leaderboard from server.");
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
                SubscribeToShowLeaderBoardEvent();
                return Task.CompletedTask;
            };

            hubConnection.Closed += async error =>
            {
                Debug.LogError("Connection closed. Attempting to reconnect...");
                // await Task.Delay(new Random().Next(0, 5) * 1000);
                // await Task.Delay(3000);
                await hubConnection.StartAsync();
                // SubscribeToShowLeaderBoardEvent();
                // await hubConnection.InvokeAsync("ShowLeaderBoard", StaticData.Username);
            };
        }

        private void SubscribeToShowLeaderBoardEvent()
        {
            Debug.Log("Subscribing to ShowLeaderBoard event...");

            hubConnection.On<string>("ShowLeaderBoard", (message) =>
            {
                Debug.Log("Show leaderboard from SignalR success");

                StaticData.IsSessionDone = true;
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    Debug.Log("Loading scene 'RF Castle/Scenes/Quang'");
                    SceneManager.LoadScene("RF Castle/Scenes/Quang");
                });
            });
        }
    }
}
