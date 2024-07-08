using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Home
{
    public class HomeManager : MonoBehaviour
    {
        public static HomeManager Instance;
        // public event Action RoomStarted;

        private HubConnection _connection;
        private const string ServerAddress = "https://escaperoom.ddnsking.com";

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

                await ProccessUpdateRoomListAsync();
            }
        }

        public async Task<List<GameSessionDto>> GetRooms()
        {
            return await APIManager.Instance.GetRoomsAsync();
        }

        public async Task JoinRoomByCode(string roomCode)
        {
            gameSession = await APIManager.Instance.JoinRoomByCodeAsync(roomCode);

            if (gameSession is not null)
            {
                waitRoom.ResetReadyButton();
                waitRoom.UpdateStates();
                homeCanvas.ShowObject("WaitRoom");

                // Process find room
                await ProcessFindOrReadyOrExistRoomAsync();
                await ProccessUpdateRoomListAsync();
            }
        }

        public async Task JoinRoomBySelect(int sessionId)
        {
            gameSession = await APIManager.Instance.JoinRoomBySelectAsync(sessionId);

            if (gameSession is not null)
            {
                waitRoom.ResetReadyButton();
                waitRoom.UpdateStates();
                homeCanvas.ShowObject("WaitRoom");

                // Process find room
                await ProcessFindOrReadyOrExistRoomAsync();
                await ProccessUpdateRoomListAsync();
            }
        }

        public async Task Login(LoginBody requestBody)
        {
            var success = await APIManager.Instance.LoginAsync(requestBody);
            if (success)
            {
                StaticData.Username = requestBody.Username;
                homeCanvas.ShowObject("MainButtons");
                // homeCanvas.ShowObject("FindRoomMenu");
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

        private async Task ProccessUpdateRoomListAsync()
        {
            await _connection.InvokeAsync("InvokeRoomCreateAsync", StaticData.Username);
        }

        public async Task FindRandomRoom()
        {
            gameSession = await APIManager.Instance.FindRandomRoomAsync();

            if (gameSession is not null)
            {
                waitRoom.ResetReadyButton();
                waitRoom.UpdateStates();
                homeCanvas.ShowObject("WaitRoom");

                // Process find room
                await ProcessFindOrReadyOrExistRoomAsync();
                await ProccessUpdateRoomListAsync();
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

            await ProccessUpdateRoomListAsync();
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

            await ProccessUpdateRoomListAsync();
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

        public async Task ConnectSignalRServer()
        {
            try
            {
                // Config signalR connection
                _connection = new HubConnectionBuilder().WithUrl($"{ServerAddress}/start-room").Build();

                _connection.On<string>("InvokeConnectionMessage", Debug.Log);

                _connection.On<bool, double, int>("OnStartingProcessed", (_, endTime, sessionId) =>
                {
                    Debug.Log("Invoke start room for other");

                    if (gameSession is not null)
                    {
                        // Only process start game for users in same room with host user
                        if (gameSession.SessionId == sessionId)
                        {
                            StaticData.RemainTime = (float)endTime;
                            Debug.Log(1000);
                            // Hm nay loi r H, mai hoi lai V xem vi connection vs call thanh cong ma k chuyen sceene
                            UnityMainThreadDispatcher.Instance().Enqueue(() =>
                            {
                                SceneManager.LoadScene("RF Castle/Scenes/MH1");
                                Debug.Log(1001);
                            });
                            Debug.Log(1002);
                        }
                    }
                });

                _connection.On<int, int, int, int>("OnTriggerInWaitingRoomProcessed",
                    (totalPlayerInSession, sessionPlayerCap, totalReadyPlayers, gameSessionId) =>
                    {
                        if (gameSession is not null)
                        {
                            // Only process start game for users in same room with host user
                            if (gameSession.SessionId == gameSessionId)
                            {
                                waitRoom.ProcessFindOrReadyUpdate(totalPlayerInSession, sessionPlayerCap,
                                    totalReadyPlayers);
                            }
                        }
                    });

                _connection.On<int, int, int, int>("OnTriggerJoinRoomProcessed",
                (totalPlayerInSession, sessionPlayerCap, totalReadyPlayers, gameSessionId) =>
                {
                    if (gameSession is not null)
                    {
                        // Only process start game for users in same room with host user
                        if (gameSession.SessionId == gameSessionId)
                        {
                            waitRoom.ProcessFindOrReadyUpdate(totalPlayerInSession, sessionPlayerCap,
                                totalReadyPlayers);
                        }
                    }
                });

                // Start        
                await _connection.StartAsync();
                StaticData.HubConnection = _connection;
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }
    }
}