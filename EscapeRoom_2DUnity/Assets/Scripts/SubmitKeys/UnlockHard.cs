using System;
using System.Collections;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SubmitKeys
{
    public class UnlockHard : MonoBehaviour
    {
        [SerializeField] private TMP_InputField input1; // Part of the Key 1
        [SerializeField] private TMP_InputField input2; // Part of the Key 2
        [SerializeField] private TMP_InputField input3; // Part of the Key 3
        [SerializeField] private TMP_InputField input4; // Part of the Key 4
        [SerializeField] private Button sendButton;
        [SerializeField] private GameObject uiPanel;
        [SerializeField] private GameObject errorMessage;
        [SerializeField] public AudioSource correctSound;
        [SerializeField] public AudioSource incorrectSound;

        [SerializeField] public GameObject LeaderBoard;

        private Collider2D _collisionCollider;
        private bool _isUiVisible;

        private void Start()
        {
            errorMessage.SetActive(false);
            uiPanel.SetActive(false);
            sendButton.onClick.AddListener(OnSendButtonClick);
            LeaderBoard.SetActive(false);

            if (StaticData.IsSessionDone)
            {
                LeaderBoard.SetActive(true);
                LeaderBoard.GetComponent<ScoreBoard.ScoreBoard>().LoadAndDisplayLeaderboard();
            }
        }

        public void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Unlock") && !_isUiVisible)
            {
                Debug.Log("Unlock here");
                collision.isTrigger = false;
                _collisionCollider = collision;
                uiPanel.SetActive(true);
                _isUiVisible = true;
            }

            if (collision.CompareTag("Passed"))
            {
                uiPanel.SetActive(false);
            }

            if (collision.CompareTag("NextRoom"))
            {
                Debug.Log("Unlock scenes");
                SceneManager.LoadScene("RF Castle/Scenes/Quang");
            }

            if (collision.CompareTag("GoToNhaRoom"))
            {
                SceneManager.LoadScene("RF Castle/Scenes/MH2");
            }
        }

        public void OnSendButtonClick()
        {
            string value1 = input1.text;
            string value2 = input2.text;
            string value3 = input3.text;
            string value4 = input4.text;
            SendData(value1, value2, value3, value4);
        }

        public void SendData(string value1, string value2, string value3, string value4)
        {
            string username = StaticData.Username;
            string key = string.Concat(value1, value2, value3, value4);
            string isHard = "true";
            string url = $"https://localhost:7000/api/players/{username}/room/unclock/{key}?isHard={isHard}";
            Debug.Log("Request URL: " + url);
            StartCoroutine(GetRequest(url));
        }

        private IEnumerator GetRequest(string url)
        {
            UnityWebRequest request = UnityWebRequest.Get(url);
            request.downloadHandler = new DownloadHandlerBuffer();
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Response: " + request.downloadHandler.text);
                correctSound.Play();
                _collisionCollider.isTrigger = true;
                _collisionCollider.tag = "Passed";
                HideUI();
                LeaderBoard.SetActive(true);
                LeaderBoard.GetComponent<ScoreBoard.ScoreBoard>().LoadAndDisplayLeaderboard(); // Load and display leaderboard

                var hubConnection = StaticData.HubConnection;

                if (hubConnection != null)
                {
                    // yield return new WaitForSeconds(5);
                    Task invokeTask = hubConnection.InvokeAsync("InvokeLeaderBoard", StaticData.Username);
                    yield return new WaitUntil(() => invokeTask.IsCompleted);

                    if (invokeTask.IsFaulted)
                    {
                        Debug.LogError("SignalR InvokeLeaderBoard failed: " + invokeTask.Exception?.GetBaseException().Message);
                    }
                    else
                    {
                        Debug.Log("SignalR InvokeLeaderBoard called successfully.");
                    }
                }
                else
                {
                    Debug.LogError("HubConnection is not initialized.");
                }
            }
            else
            {
                Debug.Log("Error: " + request.error);
                incorrectSound.Play();
                errorMessage.SetActive(true);
                StartCoroutine(HideErrorMessageAfterDelay(5.0f));
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                string value1 = input1.text;
                string value2 = input2.text;
                string value3 = input3.text;
                string value4 = input4.text;
                SendData(value1, value2, value3, value4);
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                HideUI();
            }
        }

        private void HideUI()
        {
            Debug.Log("HideUi");
            if (uiPanel != null)
            {
                uiPanel.SetActive(false);
                _isUiVisible = false;
                if (_collisionCollider != null && _collisionCollider.CompareTag("Unlock"))
                {
                    _collisionCollider.isTrigger = true;
                    _collisionCollider.tag = "Unlock";
                }
                Debug.Log("UI Panel is hidden.");
            }
            else
            {
                Debug.LogError("UI Panel is not assigned.");
            }
        }

        private IEnumerator HideErrorMessageAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            errorMessage.SetActive(false);
        }
    }
}
