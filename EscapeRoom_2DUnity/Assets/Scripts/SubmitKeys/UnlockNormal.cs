using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UnlockNormal : MonoBehaviour
{
    [SerializeField] private TMP_InputField input1; // Part of the Key 1
    [SerializeField] private TMP_InputField input2; // Part of the Key 2
    [SerializeField] private TMP_InputField input3; // Part of the Key 3
    [SerializeField] private TMP_InputField input4; // Part of the Key 4
    [SerializeField] private Button sendButton;
    // [SerializeField] float destroyDelay = 0.5f;
    [SerializeField] private GameObject uiPanel;
    [SerializeField] private GameObject errorMessage;
    [SerializeField] public AudioSource correctSound; // Add this line
    [SerializeField] public AudioSource incorrectSound;
    private Collider2D _collisionCollider;
    private bool _isUiVisible;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Unlock") && !_isUiVisible)
        {
            Debug.Log("Unlock here");

            // ??t isTrigger thành true cho collider c?a ??i t??ng va ch?m
            collision.isTrigger = false;

            _collisionCollider = collision;

            // Set hi?n Ui
            uiPanel.SetActive(true);
            _isUiVisible = true;
        }

        if (collision.CompareTag("Passed"))
        {
            uiPanel.SetActive(false);
        }
    }

    private void Start()
    {
        errorMessage.SetActive(false);
        uiPanel.SetActive(false);
        // Add listener to the button to call SendData when clicked
        sendButton.onClick.AddListener(OnSendButtonClick);
    }

    public void OnSendButtonClick()
    {
        string value1 = input1.text;
        string value2 = input2.text;
        string value3 = input3.text;
        string value4 = input4.text;

        SendData(value1, value2, value3, value4);
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public void SendData(string value1, string value2, string value3, string value4)
    {
        string username = StaticData.Username;
        string key = string.Concat(value1, value2, value3, value4);
        string isHard = "false";

        string url = $"http://localhost:6000/api/players/{username}/room/unclock/{key}?isHard={isHard}";
        Debug.Log("Request URL: " + url);
        StartCoroutine(GetRequest(url));
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private IEnumerator GetRequest(string url)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.downloadHandler = new DownloadHandlerBuffer();

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Response: " + request.downloadHandler.text);
            correctSound.Play();
            _collisionCollider.isTrigger = true;
            _collisionCollider.tag = "Passed";
            HideUI();
            SceneManager.LoadScene("RF Castle/Scenes/Quang");
        }
        else
        {
            Debug.Log("Error: " + request.error);
            incorrectSound.Play();
            errorMessage.SetActive(true);
            StartCoroutine(HideErrorMessageAfterDelay(5.0f));
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return)) // Call SendData when the Return key is pressed
        {
            string value1 = input1.text;
            string value2 = input2.text;
            string value3 = input3.text;
            string value4 = input4.text;

            SendData(value1, value2, value3, value4);
        }

        // Hide UI when the Escape key is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HideUI();
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void HideUI()
    {
        Debug.Log("HideUi");
        if (uiPanel != null)
        {
            uiPanel.SetActive(false);
            _isUiVisible = false;
            // Reset the collider state to allow re-triggering
            if (_collisionCollider != null && _collisionCollider.CompareTag("Unlock"))
            {
                _collisionCollider.isTrigger = true; // reset to allow future triggering
                _collisionCollider.tag = "Unlock";  // reset to original tag
            }
            Debug.Log("UI Panel is hidden.");
        }
        else
        {
            Debug.LogError("UI Panel is not assigned.");
        }
    }
    private IEnumerator HideErrorMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        errorMessage.SetActive(false);
    }
}