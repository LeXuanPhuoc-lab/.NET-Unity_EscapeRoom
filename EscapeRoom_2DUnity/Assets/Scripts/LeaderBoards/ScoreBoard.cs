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

        [SerializeField] private Transform highScoreHolderTransform = null;
        [SerializeField] private GameObject scoreBoardEntryObject = null;

        private ScoreBoardSaveData saveData = new ScoreBoardSaveData();
        // viet vao file json
        private void Start()
        {
            /*ScoreBoardSaveData saveScore = GetSaveScore();
            UpdateUI(saveScore);*/

            var scoreBoard = FindObjectOfType<ScoreBoard>();
            scoreBoard.AddScore("Alice", 10);
            scoreBoard.AddScore("Bob", 8);
            scoreBoard.AddScore("Charlie", 15);
        }
        /*   public void AddEntry(ScoreBoardEntryData scoreBoardEntryData)
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
           }*/
        /*   private void UpdateUI(ScoreBoardSaveData saveScore)
           {
               foreach (Transform child in highScoreHolderStranfrom) { 
               Destroy(child.gameObject); 
               }
               foreach (ScoreBoardEntryData highscore in saveScore.highScore) {
                   Instantiate(ScoreBoardEntryObject, highScoreHolderStranfrom).GetComponent<ScoreBoardEntryUI>().Initialise(highscore);

               }

           }*/

        public void AddScore(string username, int amountOfAnswer)
        {
            int top = saveData.highScores.Count + 1; // Xác ??nh th? t? m?i
            var newEntry = new ScoreBoardEntryData(top, username, amountOfAnswer);
            saveData.highScores.Add(newEntry);
            UpdateUI();
        }
        public void UpdateUI()
        {
            // Xóa các m?c hi?n t?i
            foreach (Transform child in highScoreHolderTransform)
            {
                Destroy(child.gameObject);
            }

            // T?o các m?c m?i
            foreach (ScoreBoardEntryData highscore in saveData.highScores)
            {
                var entryObject = Instantiate(scoreBoardEntryObject, highScoreHolderTransform);
                var entryUI = entryObject.GetComponent<ScoreBoardEntryUI>();
                if (entryUI != null)
                {
                    entryUI.Initialise(highscore);
                }
            }
        }
        [ContextMenu ("add TestEntry ")]
      /*  public void addTestEntry() {
            AddEntry(TestEntryData);
        }*/

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
