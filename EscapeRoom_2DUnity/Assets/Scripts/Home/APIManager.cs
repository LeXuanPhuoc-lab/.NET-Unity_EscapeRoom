using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Home
{
    public class APIManager : MonoBehaviour
    {
        public APIManager()
        {
        }

        public IEnumerator CreateRoomAsync(CreateRoomBody body)
        {
            Debug.Log(3);
            // Create the request body
            string requestBody =
                $"{{\"username\":\"${body.Username}\",\"roomName\":\"${body.RoomName}\",\"totalPlayer\":${body.TotalPlayer},\"endTimeToMinute\":${body.EndTimeToMinute}}}";

            // Create a UnityWebRequest object
            UnityWebRequest request = new UnityWebRequest("http://localhost:6000/api/players/room", "POST");

            // Set the request headers
            request.SetRequestHeader("Content-Type", "application/json");

            // Convert the request body to byte array and attach it to the request
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(requestBody);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();

            // Send the request asynchronously
            yield return request.SendWebRequest();

            // Check for errors
            if (request.result != UnityWebRequest.Result.Success)
            {
                // var a = JsonUtility.FromJson<ApiResponse>(request.error);
                Debug.LogError("Error: " + request.error);
                Debug.LogError("Error2: " + request.downloadHandler.text);
            }
            else
            {
                // Request successful, handle response data
                string responseData = request.downloadHandler.text;
                Debug.Log("Response: " + responseData);

                // Parse the JSON response and extract GameSessionDto data
                GameSessionDto gameSession = JsonUtility.FromJson<GameSessionDto>(responseData);
                if (gameSession != null)
                {
                    HomeManager.Instance.gameSession = gameSession;
                    Debug.Log(HomeManager.Instance.gameSession.SessionName);
                    Debug.Log(HomeManager.Instance.gameSession.TotalPlayer);
                    Debug.Log(HomeManager.Instance.gameSession.EndTime);
                }
                else
                {
                    Debug.LogError("Failed to parse GameSessionDto from response.");
                }
            }
        }
    }
// Define a class to represent the JSON data


    [System.Serializable]
    public class ApiResponse
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