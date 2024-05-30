using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using Unity.VisualScripting;

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
        private void Start()
        {
      /*      // Tạo một mục mới cho bảng xếp hạng
            ScoreBoardEntryData newEntry = new ScoreBoardEntryData("Alice", 100);

            // Bây giờ bạn có thể thêm mục này vào danh sách các mục nhập bảng xếp hạng
            AddEntry(newEntry);*/
            ScoreBoardSaveData saveScore = GetSaveScore();
            UpdateUI(saveScore);
            SaveScore(saveScore);
      

        }
        public void AddEntry(ScoreBoardEntryData scoreBoardEntryData)
        {
            ScoreBoardSaveData savedScore = GetSaveScore();
            bool scoreAdd = false;
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
