using TMPro;
using UnityEngine;

namespace Home
{
    public class HomeCanvas : MonoBehaviour
    {
        //TODO: Can refactor to using Observer Pattern for GameObjects about show
        [SerializeField] private GameObject mainButtons;
        [SerializeField] private GameObject createRoomForm;
        [SerializeField] private GameObject waitRoom;
        [SerializeField] private TMP_Text errorMessage;

        private void Start()
        {
            mainButtons.SetActive(true);
            createRoomForm.SetActive(false);
            waitRoom.SetActive(false);
            errorMessage.text = "";
        }

        public void ShowCreateRoomForm()
        {
            mainButtons.SetActive(false);
            waitRoom.SetActive(false);
            createRoomForm.SetActive(true);
        }

        public void ShowHomeMenu()
        {
            mainButtons.SetActive(true);
            waitRoom.SetActive(false);
            createRoomForm.SetActive(false);
        }

        public void ShowWaitRoom()
        {
            Debug.Log(10);
            mainButtons.SetActive(false);
            waitRoom.SetActive(true);
            createRoomForm.SetActive(false);
        }

        public void ShowError(string message)
        {
            Debug.Log(message);
            errorMessage.text = message;
        }
    }
}