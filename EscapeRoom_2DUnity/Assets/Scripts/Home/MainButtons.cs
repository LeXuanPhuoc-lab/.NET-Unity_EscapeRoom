using UnityEngine;

namespace Home
{
    public class MainButtons : MonoBehaviour
    {
        public void HandleFindRoom()
        {
            HomeManager.Instance.FindRoom();
        }
    }
}