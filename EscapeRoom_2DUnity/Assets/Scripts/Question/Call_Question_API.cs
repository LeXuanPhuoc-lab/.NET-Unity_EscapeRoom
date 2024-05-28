using System.Collections;
using System.Collections.Generic;
using Models;
using UnityEngine;
using Newtonsoft.Json;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

public class Call_Question_API : MonoBehaviour
{
    public class Question
    {
        public string QuestionId { get; set; } = string.Empty;
        public string QuestionDesc { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public int? KeyDigit { get; set; }
        public virtual ICollection<QuestionAnswer> QuestionAnswers { get; set; } = new List<QuestionAnswer>();
    }

    public class QuestionAnswer
    {
        public string Answer { get; set; } = string.Empty;
        public bool IsTrue { get; set; }
    }

    [SerializeField] private TMP_Text text;
    [SerializeField] private Button answerA;
    [SerializeField] private Button answerB;
    [SerializeField] private Button answerC;
    [SerializeField] private Button answerD;
    [SerializeField] private RawImage imageBackground;
    [SerializeField] private GameObject questionScreen;
    [SerializeField] private GameObject player;

    public static bool isQuestionScreenActive = false;
    private string currentItemID = string.Empty;

    private Dictionary<string, Question> questionsDictionary = new Dictionary<string, Question>();

    void Start()
    {
        questionScreen.SetActive(false);
    }

    void Update()
    {
        if (isQuestionScreenActive && Input.GetKeyDown(KeyCode.Escape))
        {
            HideQuestionScreen();
        }
    }

    public void ShowQuestionScreen(string itemID)
    {
        currentItemID = itemID;

        if (questionsDictionary.ContainsKey(currentItemID))
        {
            var question = questionsDictionary[currentItemID];
            DisplayQuestion(question);
        }
        else
        {
            StartCoroutine(GetRequest($"http://localhost:6000/api/questions/hard-level?username=test"));
        }
    }

    public void HideQuestionScreen()
    {
        questionScreen.SetActive(false);
        isQuestionScreenActive = false;
        Time.timeScale = 1;
        player.GetComponent<Player.Player>().enabled = true;
    }

    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                webRequest.result == UnityWebRequest.Result.DataProcessingError)
            {
                Debug.LogError("Error: " + webRequest.error);
            }
            else if (webRequest.result == UnityWebRequest.Result.Success)
            {
                var json = webRequest.downloadHandler.text;
                var baseResponse = JsonConvert.DeserializeObject<BaseResponse<Question>>(json);
                Debug.Log(baseResponse.StatusCode);
                if (baseResponse.StatusCode == 200 && baseResponse.Data != null)
                {
                    Question question = baseResponse.Data;
                    Debug.Log($"QuestionDesc: {question.QuestionDesc}");
                    Debug.Log($"Image: {question.Image}");

                    questionsDictionary[currentItemID] = question;
                    DisplayQuestion(question);
                }
            }
        }
    }

    void DisplayQuestion(Question question)
    {
        ResetQuestionScreen();
        if (!string.IsNullOrWhiteSpace(question.Image))
        {
            StartCoroutine(LoadImage(question.Image));
        }

        ShowQuestion(question.QuestionDesc);
        SetAnswers(question.QuestionAnswers);
        questionScreen.SetActive(true);
        isQuestionScreenActive = true;
        Time.timeScale = 0;
        player.GetComponent<Player.Player>().enabled = false;
    }

    IEnumerator LoadImage(string url)
    {
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                imageBackground.texture = texture;
                ShowImage();
            }
            else
            {
                Debug.LogError("Failed to load image: " + www.error);
            }
        }
    }

    void ShowImage()
    {
        imageBackground.gameObject.SetActive(true);
    }

    void ShowQuestion(string questionDesc)
    {
        text.gameObject.SetActive(true);
        text.text = questionDesc;
    }

    void SetAnswers(ICollection<QuestionAnswer> questionAnswers)
    {
        var answers = new List<Button> { answerA, answerB, answerC, answerD };
        int i = 0;

        foreach (var answer in questionAnswers)
        {
            if (i < answers.Count)
            {
                answers[i].GetComponentInChildren<TMP_Text>().text = answer.Answer;
                answers[i].gameObject.SetActive(true);
                i++;
            }
        }
    }

    void ResetQuestionScreen()
    {
        text.gameObject.SetActive(false);
        imageBackground.gameObject.SetActive(false);
        answerA.gameObject.SetActive(false);
        answerB.gameObject.SetActive(false);
        answerC.gameObject.SetActive(false);
        answerD.gameObject.SetActive(false);
    }
}