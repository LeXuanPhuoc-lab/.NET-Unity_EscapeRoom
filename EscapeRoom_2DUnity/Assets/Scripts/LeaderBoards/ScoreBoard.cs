using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

namespace ScoreBoard {
    public class ScoreBoard : MonoBehaviour
    {
        [SerializeField] private int maxLeadeBoard = 5;
        [SerializeField] private Transform highScoreHolderStranfrom = null;
        [SerializeField] private GameObject ScoreBoardEntryObject = null;

        [Header("TEST")]
        [SerializeField] ScoreBoardEntryData TestEntryData = new ScoreBoardEntryData();

        private string Savepath => $"{Application.persistentDataPath}/highScores.json";

        private void Start()
        {
            ScoreBoardSaveData 
        }

    }

}
