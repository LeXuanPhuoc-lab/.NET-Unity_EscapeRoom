using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ScoreBoard
{
    [Serializable]
    public struct ScoreBoardEntryData 
    {
        public int Top;
        public string entryName;
        public int entryScore;
        public ScoreBoardEntryData(int top, string username, int amountOfAnswer)
        {
            Top = top;
            entryName = username;
            entryScore = amountOfAnswer;
        }
    }

}

