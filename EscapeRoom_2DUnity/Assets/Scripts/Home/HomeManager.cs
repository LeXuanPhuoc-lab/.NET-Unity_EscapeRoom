using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Home
{
    public class HomeManager : MonoBehaviour
    {
        public static HomeManager Instance;

        [HideInInspector] public GameSessionDto gameSession;
        [SerializeField] private HomeCanvas homeCanvas;
        [SerializeField] private WaitRoom waitRoom;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public async Task CreateRoom(CreateRoomBody requestBody)
        {
            Debug.Log(2);
            requestBody.Username = StaticData.Username;
            Debug.Log(3);
            gameSession = await APIManager.Instance.CreateRoomAsync(requestBody);
            Debug.Log(7);

            if (gameSession is not null)
            {
                waitRoom.ResetReadyButton();
                waitRoom.UpdateStates();
                Debug.Log(9);
                homeCanvas.ShowWaitRoom();
                Debug.Log(11);
            }
        }

        public async Task FindRoom()
        {
            gameSession = await APIManager.Instance.FindRoomAsync(StaticData.Username);
            if (gameSession is not null)
            {
                waitRoom.ResetReadyButton();
                waitRoom.UpdateStates();
                Debug.Log(9);
                homeCanvas.ShowWaitRoom();
                Debug.Log(11);
            }
        }

        public async Task StartRoom()
        {
            var success = await APIManager.Instance.StartRoomAsync(StaticData.Username);
            if (success)
            {
                StaticData.RemainTime = (float)gameSession.EndTime.TotalSeconds;
                SceneManager.LoadScene("RF Castle/Scenes/MH1");
            }
        }

        public void ShowError(string message)
        {
            homeCanvas.ShowError(message);
        }

        public async Task OutRoom()
        {
            var success = await APIManager.Instance.OutRoomAsync(StaticData.Username);
            if (success)
            {
                homeCanvas.ShowHomeMenu();
            }
        }

        public async Task Ready()
        {
            Debug.Log(19);
            var success = await APIManager.Instance.ReadyAsync(StaticData.Username);
            if (success)
            {
                Debug.Log(21);
                waitRoom.HandleReadySuccess();
            }

            Debug.Log(22);
        }
    }
}