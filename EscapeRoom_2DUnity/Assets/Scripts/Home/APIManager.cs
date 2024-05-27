using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
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
            }

            return response.Data;
        }
    }
// Define a class to represent the JSON data


    [System.Serializable]
    public class CreateRoomResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = null!;
        public GameSessionDto Data { get; set; } = null!;
        public bool IsSuccess { get; set; }
    }


    [System.Serializable]
    public class CreateRoomBody
    {
        public string Username { get; set; } = null!;
        public string RoomName { get; set; } = null!;
        public int TotalPlayer { get; set; }
        public int EndTimeToMinute { get; set; }
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
        public string SessionName { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public int TotalPlayer { get; set; }

        public bool IsWaiting { get; set; }

        public bool IsEnd { get; set; }
        public string Hint { get; set; } = string.Empty;
        public virtual IEnumerable<PlayerDto> Players { get; set; } = new List<PlayerDto>();
    }
}