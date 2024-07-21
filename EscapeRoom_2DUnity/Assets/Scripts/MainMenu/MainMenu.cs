using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private static MainMenu _instance = null;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayGame()
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            SceneManager.LoadScene("RF Castle/Scenes/Home");
        });
    }

    public void GoToSettingMenu()
    {
    }

    public void GoToMainMenu()
    {
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
