using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Assets.Scripts.Models;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Models;
using TMPro;
using Unity.VisualScripting;

namespace ScoreBoard
{
    public class ScoreBoard : MonoBehaviour
    {
        private int maxLeadeBoard = StaticData.TotalPlayer;
        // [SerializeField] public Transform highScoreHolderTransform = null; // Ensure this is assigned in the Inspector
        [SerializeField] public GameObject ScoreBoardEntryObject = null; // Ensure this is assigned in the Inspector
        [SerializeField] public Transform scrollView = null;

        private void Start()
        {
            gameObject.SetActive(false);
        }

        public async void LoadAndDisplayLeaderboard()
        {
            Debug.Log("Fetching leaderboard...");
            List<ScoreBoardEntryData> leaderboard = await getLeaderBoardAsync(StaticData.Username);

            if (leaderboard != null)
            {
                ScoreBoardSaveData savedScore = new ScoreBoardSaveData
                {
                    highScore = leaderboard
                };

                UpdateUI(savedScore);
                SaveScore(savedScore);
            }
        }

        public async Task<List<ScoreBoardEntryData>> getLeaderBoardAsync(string usernamePlayer)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get($"https://escaperoom.ddnsking.com/api/leaderboard?username={usernamePlayer}"))
            {
                var operation = webRequest.SendWebRequest();

                while (!operation.isDone)
                {
                    await Task.Yield();
                }

                if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                    webRequest.result == UnityWebRequest.Result.DataProcessingError)
                {
                    Debug.LogError("Lỗi: " + webRequest.error);
                    return null;
                }
                else if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    var json = webRequest.downloadHandler.text;

                    var responseParam = JsonConvert.DeserializeObject<BaseResponse<List<LeaderBoardResponse>>>(json);

                    if (responseParam.StatusCode == 200 && responseParam.Data != null)
                    {
                        List<ScoreBoardEntryData> leaderboardEntries = new List<ScoreBoardEntryData>();

                        foreach (var entryJson in responseParam.Data)
                        {
                            ScoreBoardEntryData entryData = new ScoreBoardEntryData(
                                entryJson.LeaderBoardId,
                                entryJson.Player.Username,
                                entryJson.TotalRightAnswer
                            );

                            leaderboardEntries.Add(entryData);
                        }

                        return leaderboardEntries;
                    }
                    else
                    {
                        Debug.LogError("Lỗi: " + responseParam.Message);
                        return null;
                    }
                }
            }
            return null;
        }

        private void UpdateUI(ScoreBoardSaveData saveScore)
        {
            foreach (Transform child in scrollView)
            {
                Destroy(child.gameObject);
            }

            for (int i = 0; i < saveScore.highScore.Count; i++)
            {
                var highscore = saveScore.highScore[i];
                // var entryObject = Instantiate(ScoreBoardEntryObject, highScoreHolderTransform);
                var entryObject = Instantiate(ScoreBoardEntryObject);
                var entryUI = entryObject.GetComponent<ScoreBoardEntryUI>();

                entryUI.transform.SetParent(scrollView.transform, false);
                entryUI.Initialise(highscore, i + 1);
            }
        }


        private ScoreBoardSaveData GetSaveScore()
        {
            if (!File.Exists(Savepath))
            {
                File.Create(Savepath).Dispose();
                return new ScoreBoardSaveData();
            }

            using (StreamReader stream = new StreamReader(Savepath))
            {
                string json = stream.ReadToEnd();
                return JsonUtility.FromJson<ScoreBoardSaveData>(json);
            }
        }

        private void SaveScore(ScoreBoardSaveData scoreSaveData)
        {
            using (StreamWriter stream = new StreamWriter(Savepath))
            {
                string json = JsonUtility.ToJson(scoreSaveData, true);
                stream.Write(json);
            }
        }

        private string Savepath => $"{Application.persistentDataPath}/highScores.json";
    }
}
