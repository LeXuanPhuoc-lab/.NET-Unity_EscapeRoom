using System;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Home
{
    public class WaitRoom : MonoBehaviour
    {
        [SerializeField] private TMP_Text roomText;
        [SerializeField] private TMP_Text totalPlayerText;
        [SerializeField] private TMP_Text durationText;
        // Start is called before the first frame update

        public void UpdateStates()
        {
            Debug.Log(8);
            roomText.text = $"Room: ${HomeManager.Instance.gameSession.SessionName}";
            totalPlayerText.text =
                $"Total Player: ${HomeManager.Instance.gameSession.Players.Count()}/${HomeManager.Instance.gameSession.TotalPlayer}";
            durationText.text = $"Duration: ${HomeManager.Instance.gameSession.EndTime.ToString()}";
            Debug.Log(8.1);
        }
    }
}