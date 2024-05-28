using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using LeastSquares;
using Steamworks.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LeastSquares
{
    /// <summary>
    /// Script to fill the leaderboard UI from the Steam leaderboard
    /// </summary>
    public class LeaderboardUI : MonoBehaviour
    {
        public int EntriesToShowAtOnce = 100;
        public GameObject EntryPrefab;
        public TMP_InputField Input;
        public SteamLeaderboard Leaderboard;
        public LeaderboardType Type = LeaderboardType.Global;
        private List<GameObject> _rows = new ();
        private int _offset;

        void Start()
        {
            RefreshScores();
        }

        /// <summary>
        /// Fill the leaderboardUI with new scores
        /// </summary>
        async void RefreshScores()
        {
            LeaderboardEntry[] scores;
            switch (Type)
            {
                case LeaderboardType.Global:
                    scores = await Leaderboard.GetScores(EntriesToShowAtOnce-1, 1 + _offset);
                    break;
                case LeaderboardType.Friends:
                    scores = await Leaderboard.GetScoresFromFriends();
                    break;
                case LeaderboardType.AroundUser:
                    scores = await Leaderboard.GetScoresAroundUser(EntriesToShowAtOnce / 2);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            RegenerateUI(scores);
        }

        /// <summary>
        /// Renegenerate the leaderboard rows
        /// </summary>
        /// <param name="scores">An array of leaderboard entries</param>
        async void RegenerateUI(LeaderboardEntry[] scores)
        {
            var oldRows = _rows;
            _rows = new List<GameObject>();
            for (var i = 0; i < scores.Length; i++)
            {
                var go = await CreateRow(scores[i]);

                _rows.Add(go);
            }

            for (var i = 0; i < oldRows.Count; i++)
            {
                Destroy(oldRows[i]);
            }
        }

        /// <summary>
        /// Create a row for the leaderboard entry
        /// </summary>
        /// <param name="entry">The given LeaderboardEntry</param>
        /// <returns>A GameObject representing the row</returns>
        private async Task<GameObject> CreateRow(LeaderboardEntry entry)
        {
            var go = Instantiate(EntryPrefab, transform);
            var row = go.GetComponent<LeaderboardUIRow>();
            row.Score.text = entry.Score.ToString();
            row.Name.text = entry.User.Name;
            row.Rank.text = entry.GlobalRank.ToString();
            var maybeImage = await entry.User.GetSmallAvatarAsync();
            if (maybeImage.HasValue)
            {
                var tex2D = maybeImage.Value.Convert();
                row.Avatar.sprite = Sprite.Create(tex2D, new Rect(0, 0, tex2D.width, tex2D.height), Vector2.zero);
            }

            return go;
        }

        /// <summary>
        /// Upload the score in the text field to the leaderboard. Called from the "Save Score" button
        /// </summary>
        public void SaveScore()
        {
            var text = Input.text;
            Leaderboard.SubmitScore(int.Parse(text));
            RefreshScores();
        }
        
    }

    [Serializable]
    public enum LeaderboardType
    {
        Global,
        Friends,
        AroundUser
    }
}