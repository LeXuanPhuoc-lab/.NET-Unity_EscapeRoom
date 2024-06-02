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
        }

        public async Task CreateRoom(CreateRoomBody requestBody)
        {
            Debug.Log(3);
            gameSession = await APIManager.Instance.CreateRoomAsync(requestBody);
            Debug.Log(7);

            if (gameSession is not null)
            {
                waitRoom.ResetReadyButton();
                waitRoom.UpdateStates();
                homeCanvas.ShowObject("WaitRoom");
            }
        }

        public async Task Login(LoginBody requestBody)
        {
            var success = await APIManager.Instance.LoginAsync(requestBody);
            if (success)
            {
                StaticData.Username = requestBody.Username;
                homeCanvas.ShowObject("MainButtons");
                Debug.Log(StaticData.Username);
            }
        }

        public async Task Register(LoginBody requestBody)
        {
            var success = await APIManager.Instance.RegisterAsync(requestBody);
            if (success)
            {
                StaticData.Username = requestBody.Username;
                homeCanvas.ShowObject("MainButtons");
                Debug.Log(StaticData.Username);
            }
        }

        private async Task ProcessFindOrReadyOrExistRoomAsync()
        {
            await _connection.InvokeAsync("InvokeFindOrReadyOrExistAsync", StaticData.Username);
        }

        public async Task FindRoom()
        {
            gameSession = await APIManager.Instance.FindRoomAsync();

            if (gameSession is not null)
            {
                waitRoom.ResetReadyButton();
                waitRoom.UpdateStates();
                homeCanvas.ShowObject("WaitRoom");

                // Process find room
                await ProcessFindOrReadyOrExistRoomAsync();
            }
        }

        private async Task ProcessStartRoom()
        {
            await _connection.InvokeAsync("InvokeStartAsync", StaticData.Username);
        }

        public async Task StartRoom()
        {
            var success = await APIManager.Instance.StartRoomAsync();

            if (success)
            {
                await ProcessStartRoom();
            }
        }

        public void ShowError(string message)
        {
            homeCanvas.ShowError(message);
        }

        public async Task OutRoom()
        {
            var success = await APIManager.Instance.OutRoomAsync();
            if (success)
            {
                Debug.Log("Out Successfully");
                homeCanvas.ShowObject("MainButtons");
                await ProcessFindOrReadyOrExistRoomAsync();
            }
        }

        public async Task Ready()
        {
            Debug.Log(19);
            var success = await APIManager.Instance.ReadyAsync();
            if (success)
            {
                // Process ready 

                Debug.Log(21);
                homeCanvas.SetDefaultErrorMessage();
                waitRoom.HandleReadySuccess();
                await ProcessFindOrReadyOrExistRoomAsync();
            }

            Debug.Log(22);
        }

        public async Task ConnnectSignalRServer()
        {
            try
            {
                // Config signalR connection
                _connection = new HubConnectionBuilder().WithUrl($"{ServerAddress}/start-room").Build();

                _connection.On<string>("InvokeConnectionMessage", (message) => { Debug.Log(message); });

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

                _connection.On<int, int, int>("OnTriggerInWaitingRoomProcessed",
                    (totalPlayerInSession, sessionPlayerCap, totalReadyPlayers) =>
                    {
                        waitRoom.ProcessFindOrReadyUpdate(totalPlayerInSession, sessionPlayerCap, totalReadyPlayers);
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