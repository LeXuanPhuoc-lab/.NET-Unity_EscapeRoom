using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Unlock : MonoBehaviour
{
    [SerializeField] private TMP_InputField Input1; // Part of the Key 1
    [SerializeField] private TMP_InputField Input2; // Part of the Key 2
    [SerializeField] private TMP_InputField Input3; // Part of the Key 3
    [SerializeField] private TMP_InputField Input4; // Part of the Key 4
    [SerializeField] private Button sendButton;
    [SerializeField] float destroyDelay = 0.5f;
    [SerializeField] private GameObject uiPanel;
    private Collider2D collisionCollider;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Unlock")
        {
            Debug.Log("Unlock here");

            // Đặt isTrigger thành true cho collider của đối tượng va chạm
            collision.isTrigger = false;

            collisionCollider = collision;

            // Set hiện Ui
            uiPanel.SetActive(true);
        }

        if (collision.tag == "Passed")
        {
            uiPanel.SetActive(false);
        }
    }

    private void Start()
    {
        uiPanel.SetActive(false);
        // Add listener to the button to call SendData when clicked
        sendButton.onClick.AddListener(OnSendButtonClick);
    }

    public void OnSendButtonClick()
    {
        string value1 = Input1.text;
        string value2 = Input2.text;
        string value3 = Input3.text;
        string value4 = Input4.text;

        SendData(value1, value2, value3, value4);
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public void SendData(string value1, string value2, string value3, string value4)
    {
        string username = "phuoc";
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
            collisionCollider.isTrigger = true;
            collisionCollider.tag = "Passed";
            HideUI();
        }
        else
        {
            Debug.Log("Error: " + request.error);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return)) // Call SendData when the Return key is pressed
        {
            string value1 = Input1.text;
            string value2 = Input2.text;
            string value3 = Input3.text;
            string value4 = Input4.text;

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
            Debug.Log("UI Panel is hidden.");
        }
        else
        {
            Debug.LogError("UI Panel is not assigned.");
        }
    }
}