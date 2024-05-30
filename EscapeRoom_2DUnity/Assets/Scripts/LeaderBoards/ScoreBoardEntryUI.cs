using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace ScoreBoard {

    public class ScoreBoardEntryUI : MonoBehaviour
    {

      /*  [SerializeField]
        private TextMeshProUGUI TopText = null;*/
        [SerializeField]
        private TextMeshProUGUI entryNameText = null;
        [SerializeField]
        private TextMeshProUGUI entryScoreText = null;
    
     public void Initialise(ScoreBoardEntryData data)
        {
           // TopText.text = data.Top.ToString();
            entryNameText.text = data.entryName;
            entryNameText.text = data.entryScore.ToString();


        }
    }
}

