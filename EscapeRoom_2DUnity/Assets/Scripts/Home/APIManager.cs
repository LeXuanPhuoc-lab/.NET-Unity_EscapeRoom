using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

namespace Home
{
    public class APIManager : MonoBehaviour
    {
        public static APIManager Instance;

        private readonly JsonSerializerSettings _serializerSettings = new();

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

            _serializerSettings.TypeNameHandling = TypeNameHandling.Auto;
            _serializerSettings.Formatting = Formatting.Indented;
        }

        public async Task<GameSessionDto> CreateRoomAsync(CreateRoomBody body)
        {
            Debug.Log(4);
            var requestBody = JsonConvert.SerializeObject(body, _serializerSettings);

            var httpClient = new HttpClient();
            var httpResponseMessage = await httpClient.PostAsync(
                "http://localhost:6000/api/players/room",
                new StringContent(requestBody, Encoding.UTF8, "application/json"));

            var serializedResponseBody = await httpResponseMessage.Content.ReadAsStringAsync();

            var response = JsonConvert.DeserializeObject<CreateRoomResponse>(serializedResponseBody);

            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                Debug.Log("Error");
                Debug.Log(response.Message);
                HomeManager.Instance.ShowError(response.Message);
                return null;
            }

            StaticData.KeyOder = response.Data.Hint;

            return response.Data;
        }

        public async Task<GameSessionDto> JoinRoomByCodeAsync(string roomCode)
        {
            var httpClient = new HttpClient();
            var httpResponseMessage = await httpClient.GetAsync(
                $"http://localhost:6000/api/game-sessions/code?username={StaticData.Username}&sessionCode={roomCode}");

            var serializedResponseBody = await httpResponseMessage.Content.ReadAsStringAsync();

            var response = JsonConvert.DeserializeObject<CreateRoomResponse>(serializedResponseBody);

            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                Debug.Log("Error");
                Debug.Log(response.Message);
                HomeManager.Instance.ShowError(response.Message);
                return null;
            }

            return response.Data;
        }

        public async Task<GameSessionDto> JoinRoomBySelectAsync(int sessionId)
        {
            var httpClient = new HttpClient();
            var httpResponseMessage = await httpClient.GetAsync(
                $"http://localhost:6000/api/game-sessions/{sessionId}?username={StaticData.Username}");

            var serializedResponseBody = await httpResponseMessage.Content.ReadAsStringAsync();

            var response = JsonConvert.DeserializeObject<CreateRoomResponse>(serializedResponseBody);

            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                Debug.Log("Error");
                Debug.Log(response);
                Debug.Log(response.Message);
                HomeManager.Instance.ShowError(response.Message);
                return null;
            }

            return response.Data;
        }

        public async Task<List<GameSessionDto>> GetRoomsAsync()
        {
            var httpClient = new HttpClient();
            var httpResponseMessage = await httpClient.GetAsync(
                $"http://localhost:6000/api/game-sessions");

            var serializedResponseBody = await httpResponseMessage.Content.ReadAsStringAsync();

            var response = JsonConvert.DeserializeObject<GetRoomsResponse>(serializedResponseBody);

            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                Debug.Log("Error");
                Debug.Log(response.Message);
                HomeManager.Instance.ShowError(response.Message);
                return new List<GameSessionDto>();
            }

            return response.Data;
        }

        public async Task<bool> LoginAsync(LoginBody body)
        {
            Debug.Log(4);
            var requestBody = JsonConvert.SerializeObject(body, _serializerSettings);

            var httpClient = new HttpClient();
            var httpResponseMessage = await httpClient.PostAsync(
                "http://localhost:6000/api/sign-in",
                new StringContent(requestBody, Encoding.UTF8, "application/json"));

            var serializedResponseBody = await httpResponseMessage.Content.ReadAsStringAsync();

            var response = JsonConvert.DeserializeObject<CreateRoomResponse>(serializedResponseBody);

            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                Debug.Log("Error");
                Debug.Log(response.Message);
                HomeManager.Instance.ShowError(response.Message);
                return false;
            }
            
            return true;
        }

        public async Task<bool> RegisterAsync(LoginBody body)
        {
            Debug.Log(4);
            var requestBody = JsonConvert.SerializeObject(body, _serializerSettings);

            var httpClient = new HttpClient();
            var httpResponseMessage = await httpClient.PostAsync(
                "http://localhost:6000/api/register",
                new StringContent(requestBody, Encoding.UTF8, "application/json"));

            var serializedResponseBody = await httpResponseMessage.Content.ReadAsStringAsync();

            var response = JsonConvert.DeserializeObject<CreateRoomResponse>(serializedResponseBody);

            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                Debug.Log("Error");
                Debug.Log(response.Message);
                HomeManager.Instance.ShowError(response.Message);
                return false;
            }

            return true;
        }

        public async Task<GameSessionDto> FindRandomRoomAsync()
        {
            var httpClient = new HttpClient();
            var httpResponseMessage = await httpClient.GetAsync(
                $"http://localhost:6000/api/players/{StaticData.Username}/room");

            var serializedResponseBody = await httpResponseMessage.Content.ReadAsStringAsync();

            var response = JsonConvert.DeserializeObject<CreateRoomResponse>(serializedResponseBody);

            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                Debug.Log("Error");
                Debug.Log(response.Message);
                HomeManager.Instance.ShowError(response.Message);
                return null;
            }

            Debug.Log("success 1");

            return response.Data;
        }

        public async Task<bool> OutRoomAsync()
        {
            var httpClient = new HttpClient();
            var httpResponseMessage = await httpClient.DeleteAsync(
                $"http://localhost:6000/api/players/{StaticData.Username}/room");

            var serializedResponseBody = await httpResponseMessage.Content.ReadAsStringAsync();

            var response = JsonConvert.DeserializeObject<CreateRoomResponse>(serializedResponseBody);

            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                Debug.Log("Error");
                Debug.Log(response.Message);
                HomeManager.Instance.ShowError(response.Message);
                return false;
            }

            return true;
        }

        public async Task<bool> ReadyAsync()
        {
            Debug.Log(20);
            var requestBody = JsonConvert.SerializeObject(new(), _serializerSettings);
            var httpClient = new HttpClient();
            Debug.Log(30);
            var httpResponseMessage = await httpClient.PutAsync(
                $"http://localhost:6000/api/players/{StaticData.Username}/room/ready",
                new StringContent(requestBody, Encoding.UTF8, "application/json"));

            Debug.Log(40);
            var serializedResponseBody = await httpResponseMessage.Content.ReadAsStringAsync();
            Debug.Log(50);
            Debug.Log(serializedResponseBody);

            var response = JsonConvert.DeserializeObject<CreateRoomResponse>(serializedResponseBody);
            Debug.Log(60);

            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                Debug.Log(70);
                Debug.Log("Error");
                Debug.Log(response.Message);
                HomeManager.Instance.ShowError(response.Message);
                return false;
            }

            Debug.Log(80);

            return true;
        }

        public async Task<bool> StartRoomAsync()
        {
            Debug.Log(20);
            var requestBody = JsonConvert.SerializeObject(new(), _serializerSettings);
            var httpClient = new HttpClient();
            Debug.Log(30);
            var httpResponseMessage = await httpClient.PutAsync(
                $"http://localhost:6000/api/players/{StaticData.Username}/room/start",
                new StringContent(requestBody, Encoding.UTF8, "application/json"));

            Debug.Log(40);
            var serializedResponseBody = await httpResponseMessage.Content.ReadAsStringAsync();
            Debug.Log(50);
            Debug.Log(serializedResponseBody);

            var response = JsonConvert.DeserializeObject<CreateRoomResponse>(serializedResponseBody);
            Debug.Log(60);

            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                Debug.Log(70);
                Debug.Log("Error");
                Debug.Log(response.Message);
                HomeManager.Instance.ShowError(response.Message);
                return false;
            }

            Debug.Log(80);

            return true;
        }
    }
    // Define a class to represent the JSON data

    [System.Serializable]
    public class LoginBody
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    [System.Serializable]
    public class CreateRoomResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = null!;
        public GameSessionDto Data { get; set; } = null!;
        public bool IsSuccess { get; set; }
        public IDictionary<string, string[]> Errors = null!;
    }

    public class GetRoomsResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = null!;
        [ItemCanBeNull] public List<GameSessionDto> Data { get; set; } = null!;
        public bool IsSuccess { get; set; }
        public IDictionary<string, string[]> Errors = null!;
    }


    [System.Serializable]
    public class CreateRoomBody
    {
        public string Username { get; set; } = null!;
        public string RoomName { get; set; } = null!;
        public int TotalPlayer { get; set; }
        public int EndTimeToMinute { get; set; }
        public string Hint { get; set; }

        public bool IsPublic { get; set; }
    }

    [System.Serializable]
    public class PlayerGameSessionDto
    {
        public int SessionId { get; set; }

        public string PlayerId { get; set; } = string.Empty;

        public bool IsHost { get; set; }

        public bool IsReady { get; set; }

        // [JsonIgnore]
        public virtual PlayerDto Player { get; set; } = null!;

        [JsonIgnore] public virtual GameSessionDto Session { get; set; } = null!;
    }


    [System.Serializable]
    public class PlayerDto
    {
        public int Id { get; set; }

        public string PlayerId { get; set; } = null!;

        public string Username { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string Email { get; set; }

        public DateTime? RegistrationDate { get; set; }

        public bool? IsActive { get; set; }
    }

    [System.Serializable]
    public class GameSessionDto
    {
        public int SessionId { get; set; }
        public string SessionName { get; set; }
        public string SessionCode { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int TotalPlayer { get; set; }
        public bool IsWaiting { get; set; }
        public bool IsEnd { get; set; }
        public string Hint { get; set; } = string.Empty;

        public virtual IEnumerable<PlayerGameSessionDto> PlayerGameSessions { get; set; } =
            new List<PlayerGameSessionDto>();
    }
}