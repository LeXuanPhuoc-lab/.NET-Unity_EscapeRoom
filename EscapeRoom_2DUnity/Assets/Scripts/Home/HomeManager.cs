using System;
using System.Collections;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Home
{
    public class HomeManager : MonoBehaviour
    {
        public static HomeManager Instance;
        public event Action RoomStarted;

        private HubConnection _connection;
        private const string ServerAddress = "https://localhost:7000";

        [HideInInspector] public GameSessionDto gameSession;
        [SerializeField] private HomeCanvas homeCanvas;
        [SerializeField] private WaitRoom waitRoom;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

            homeCanvas.SetDefaultErrorMessage();
        }

        public async Task CreateRoom(CreateRoomBody requestBody)
        {
            Debug.Log(2);
            requestBody.Username = StaticData.Username;
            Debug.Log(3);
            gameSession = await APIManager.Instance.CreateRoomAsync(requestBody);
            Debug.Log(7);

            if (gameSession is not null)
            {
                waitRoom.ResetReadyButton();
                waitRoom.UpdateStates();
                Debug.Log(9);
                homeCanvas.SetDefaultErrorMessage();
                homeCanvas.ShowWaitRoom();
                Debug.Log(11);
            }
        }

        private async Task ProcessFindOrReadyOrExistRoomAsync(string username)
        {
            await _connection.InvokeAsync("InvokeFindOrReadyOrExistAsync", username);
        }

        public async Task FindRoom()
        {
            // gameSession = await APIManager.Instance.FindRoomAsync(StaticData.Username);
            gameSession = await APIManager.Instance.FindRoomAsync("test");

            if (gameSession is not null)
            {
                
                waitRoom.ResetReadyButton();
                waitRoom.UpdateStates();
                // Debug.Log(9);
                homeCanvas.SetDefaultErrorMessage();
                homeCanvas.ShowWaitRoom();
                // Debug.Log(11);
                
                // Process find room
                await ProcessFindOrReadyOrExistRoomAsync("test");
            }
        }

        private async Task ProcessStartRoom(string username)
        {
            await _connection.InvokeAsync("InvokeStartAsync", username);
        }

        public async Task StartRoom()
        {
            var success = await APIManager.Instance.StartRoomAsync(StaticData.Username);

            if (success)
            {
                await ProcessStartRoom(StaticData.Username);
            }
        }

        public void ShowError(string message)
        {
            homeCanvas.ShowError(message);
        }

        public async Task OutRoom()
        {
            var success = await APIManager.Instance.OutRoomAsync(StaticData.Username);
            if (success)
            {
                await ProcessFindOrReadyOrExistRoomAsync("test");
                homeCanvas.SetDefaultErrorMessage();
                homeCanvas.ShowHomeMenu();
            }
        }

        public async Task Ready()
        {
            Debug.Log(19);
            // var success = await APIManager.Instance.ReadyAsync(StaticData.Username);
            var success = await APIManager.Instance.ReadyAsync("test");
            if (success)
            {
                // Process ready 
                await ProcessFindOrReadyOrExistRoomAsync("test");

                Debug.Log(21);
                homeCanvas.SetDefaultErrorMessage();
                waitRoom.HandleReadySuccess();
            }

            Debug.Log(22);
        }

        public async Task ConnnectSignalRServer()
        {
            try
            {
                // Config signalR connection
                _connection = new HubConnectionBuilder().WithUrl($"{ServerAddress}/start-room").Build();

                _connection.On<string>("InvokeConnectionMessage", (message) =>
                {
                    Debug.Log(message);
                });

                _connection.On<bool, double, int>("OnStartingProcessed", (isStarted, endTime, sessionId) =>
                {
                    Debug.Log("Invoke start room for other");

                    if (gameSession is not null)
                    {
                        // Only process start game for users in same room with host user
                        if (gameSession.SessionId == sessionId)
                        {
                            StaticData.RemainTime = (float)endTime;
                            SceneManager.LoadScene("RF Castle/Scenes/MH1");
                        }
                    }
                });

                _connection.On<int, int, int>("OnTriggerInWaitingRoomProcessed", (totalPlayerInSession,sessionPlayerCap,totalReadyPlayers) => {
                    waitRoom.ProcessFindOrReadyUpdate(totalPlayerInSession,sessionPlayerCap,totalReadyPlayers);
                });

                // Start 
                await _connection.StartAsync();
                Debug.Log("Connected with server");
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }
    }
}