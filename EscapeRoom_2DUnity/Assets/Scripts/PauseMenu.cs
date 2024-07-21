using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Home;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    // public Canvas pauseMenu;
    public static bool isPaused;
    public GameObject pauseMenu;

    // Start is called before the first frame update
    void Start()
    {
       pauseMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        // pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        // pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f; // Ensure time scale is set to normal

        // Process our game exit asynchronously
        Task result = Task.Run(async () =>
        {
            await APIManager.Instance.OutRoomAsync(); // Call async method to handle room exit
        });

        // Enqueue the scene transition on the main Unity thread
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {

            // Load the main menu scene
            // SceneManager.LoadScene("RF Castle/Scenes/Home", LoadSceneMode.Single);
            SceneManager.LoadScene("RF Castle/Scenes/Home");
        });
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
