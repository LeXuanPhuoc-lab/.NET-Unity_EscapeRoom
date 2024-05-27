using System.Collections;
using UnityEngine;

namespace Home
{
    public class HomeManager : MonoBehaviour
    {
        public static HomeManager Instance;
        private APIManager _apiManager;

        [HideInInspector] public string userName = "kingchen";
        [HideInInspector] public GameSessionDto gameSession;

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

            _apiManager = gameObject.AddComponent<APIManager>();
        }

        public void CreateRoom(CreateRoomBody requestBody)
        {
            Debug.Log(2);
            requestBody.Username = userName;
            StartCoroutine(_apiManager.CreateRoomAsync(requestBody));
        }
    }
}