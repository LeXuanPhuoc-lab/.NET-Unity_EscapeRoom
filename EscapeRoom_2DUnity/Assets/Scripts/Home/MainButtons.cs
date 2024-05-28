using UnityEngine;
using UnityEngine.SceneManagement;

namespace Home
{
    public class MainButtons : MonoBehaviour
    {
        public void HandleFindRoom()
        {
            Debug.Log($"Counter: {StaticData.RemainTime}");
            StaticData.RemainTime -= 30;
            SceneManager.LoadScene("RF Castle/Scenes/MH1");
        }
    }
}