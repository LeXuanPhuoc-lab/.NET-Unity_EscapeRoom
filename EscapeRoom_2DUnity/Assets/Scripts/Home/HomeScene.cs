using UnityEngine;

namespace Home
{
    public class HomeScene : MonoBehaviour
    {
        [SerializeField] private GameObject mainButtons;
        [SerializeField] private GameObject createRoomForm;

        private void Start()
        {
            mainButtons.SetActive(true);
            createRoomForm.SetActive(false);
        }

        public void ShowCreateRoomForm()
        {
            mainButtons.SetActive(false);
            createRoomForm.SetActive(true);
        }

        public void ShowHomeMenu()
        {
            mainButtons.SetActive(true);
            createRoomForm.SetActive(false);
        }
    }
}