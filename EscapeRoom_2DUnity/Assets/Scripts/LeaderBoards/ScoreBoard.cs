using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using Unity.VisualScripting;
using Models;
using Newtonsoft.Json;
using static Call_Question_API;
using UnityEngine.Networking;
using Home;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine.SocialPlatforms.Impl;
using SimpleJSON;
using Assets.Scripts.Models;

namespace ScoreBoard {
    public class ScoreBoard : MonoBehaviour
    {
        [SerializeField] private int maxLeadeBoard = 5;
        [SerializeField] private Transform highScoreHolderStranfrom = null;
        [SerializeField] private GameObject ScoreBoardEntryObject = null;

        [Header("TEST")]
        [SerializeField] ScoreBoardEntryData TestEntryData = new ScoreBoardEntryData();
        private string Savepath => $"{Application.persistentDataPath}/highScores.json";

        // viet vao file json
        private async void Start()
        {
            /*      // Tạo một mục mới cho bảng xếp hạng
                  ScoreBoardEntryData newEntry = new ScoreBoardEntryData("Alice", 100);

                  // Bây giờ bạn có thể thêm mục này vào danh sách các mục nhập bảng xếp hạng
                  AddEntry(newEntry);*/
            ScoreBoardSaveData saveScore = GetSaveScore();
            UpdateUI(saveScore);
            SaveScore(saveScore);
            Debug.Log("Fetching leaderboard...");

            List<ScoreBoardEntryData> leaderboard = await getLeaderBoardAsync(StaticData.Username);

            if (leaderboard != null)
            {
                // Thêm các mục từ API vào bảng xếp hạng
                foreach (var entry in leaderboard)
                {
                    AddEntry(entry);
                }
            }

        }

        // CALL APi


        public async Task<List<ScoreBoardEntryData>> getLeaderBoardAsync(string usernamePlayer)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get($"https://localhost:7000/api/leaderboard?username={usernamePlayer}"))
            {
                // Gửi yêu cầu web
                var operation = webRequest.SendWebRequest();

                // Chờ cho đến khi yêu cầu hoàn thành
                while (!operation.isDone)
                {
                    await Task.Yield();
                }

                // Kiểm tra lỗi
                if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                    webRequest.result == UnityWebRequest.Result.DataProcessingError)
                {
                    Debug.LogError("Lỗi: " + webRequest.error);
                    return null;
                }
                else if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    // Lấy phản hồi JSON
                    var json = webRequest.downloadHandler.text;
//list
                    List<ScoreBoardEntryData> leaderboardEntries = new List<ScoreBoardEntryData>();
/*                    var jsonArray = Json.Parse(json).AsArray;
*
*/
                    // Giải mã JSON thành BaseResponse<List<ScoreBoardEntryData>>
                    var responseParam = JsonConvert.DeserializeObject<Models.BaseResponse<List<LeaderBoardResponse>>>(json);

                    // Kiểm tra nếu phản hồi thành công và có dữ liệu
                    if (responseParam.StatusCode == 200 && responseParam.Data != null)
                    {
                        int id;
                        string username;
                        int totalRightAnswer;
                        //     var playerData = JsonConvert.DeserializeObject<PlayerResponse>(entryJson["player"].ToString());

                        foreach (var entryJson in responseParam.Data)
                        {
                            id = entryJson.LeaderBoardId;
                            username = entryJson.Player.Username;
                            totalRightAnswer = entryJson.TotalRightAnswer;


                            // Tạo một đối tượng ScoreBoardEntryData và thêm vào danh sách
                              ScoreBoardEntryData entryData = new ScoreBoardEntryData(id, username, totalRightAnswer);
                            AddEntry(entryData);
                        }
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


        /*      IEnumerator GetRequest(string uri)
              {
                  using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
                  {
                      yield return webRequest.SendWebRequest();

                      if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                          webRequest.result == UnityWebRequest.Result.DataProcessingError)
                      {
                          Debug.LogError("Error: " + webRequest.error);
                      }
                      else if (webRequest.result == UnityWebRequest.Result.Success)
                      {
                          var json = webRequest.downloadHandler.text;
                          var baseResponse = JsonConvert.DeserializeObject<BaseResponse<Question>>(json);
                          Debug.Log(baseResponse.StatusCode);
                          if (baseResponse.StatusCode == 200 && baseResponse.Data != null)
                          {
                              Question question = baseResponse.Data;
                              Debug.Log($"QuestionDesc: {question.QuestionDesc}");
                              Debug.Log($"Image: {question.Image}");

                              questionsDictionary[currentItemID] = question;
                              DisplayQuestion(question);
                          }
                      }
                  }
              }*/
        public void AddEntry(ScoreBoardEntryData scoreBoardEntryData)
        {
            //lay du lieu
            ScoreBoardSaveData savedScore = GetSaveScore();
            bool scoreAdd = false;

            // vong lap duyet qua danh sach diem cao 
            for(int i = 0; i<savedScore.highScore.Count ; i++) { 
            if(scoreBoardEntryData.entryScore > savedScore.highScore[i].entryScore) {
                savedScore.highScore.Insert(i, scoreBoardEntryData);
                    scoreAdd =true;
                    break;
                }
            }
            if(!scoreAdd&& savedScore.highScore.Count< maxLeadeBoard) {
                savedScore.highScore.Add(scoreBoardEntryData);
            }
            if(savedScore.highScore.Count>maxLeadeBoard)
            {
                savedScore.highScore.RemoveRange(maxLeadeBoard,savedScore.highScore.Count- maxLeadeBoard);
            }
            UpdateUI(savedScore);

            SaveScore(savedScore);
        }
        private void UpdateUI(ScoreBoardSaveData saveScore)
        {
            foreach (Transform child in highScoreHolderStranfrom) { 
            Destroy(child.gameObject); 
            }
            foreach (ScoreBoardEntryData highscore in saveScore.highScore) {
                Instantiate(ScoreBoardEntryObject, highScoreHolderStranfrom).GetComponent<ScoreBoardEntryUI>().Initialise(highscore);
            
            }

        }
        [ContextMenu ("add TestEntry ")]
        public void addTestEntry() {
            AddEntry(TestEntryData);
        }



        private ScoreBoardSaveData GetSaveScore()
        {
            if (!File.Exists(Savepath)) { 
            File.Create(Savepath).Dispose();
               return new ScoreBoardSaveData();
            }    
            using(StreamReader stream =  new StreamReader(Savepath)) {
            string json = stream.ReadToEnd();
                return JsonUtility.FromJson<ScoreBoardSaveData>(json);  
            }
        }
        private void SaveScore(ScoreBoardSaveData scoreSaveData)
        { 
            using(StreamWriter stream = new StreamWriter(Savepath))
            {
                string json= JsonUtility.ToJson(scoreSaveData,true);
                stream.Write(json);

            }
        
        
        
        }
    }

}
