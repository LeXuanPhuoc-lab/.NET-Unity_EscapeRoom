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
        public string QuestionId { get; set; }
        public string QuestionDesc { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public int? KeyDigit { get; set; }
        public virtual ICollection<QuestionAnswer> QuestionAnswers { get; set; } = new List<QuestionAnswer>();
    }

    public class QuestionAnswer
    {
        public string QuestionAnswerId { get; set; }
        public string Answer { get; set; } = string.Empty;
    }

    public class BaseResponse<T>
    {
        public int StatusCode { get; set; }
        public T Data { get; set; }
    }

    [SerializeField] private TMP_Text text;
    [SerializeField] private Button answerA;
    [SerializeField] private Button answerB;
    [SerializeField] private Button answerC;
    [SerializeField] private Button answerD;
    [SerializeField] private RawImage imageBackground;
    [SerializeField] private GameObject questionScreen;
    [SerializeField] private GameObject player;

    public int? digitKey;
    public static bool isQuestionScreenActive = false;
    private string currentItemID = string.Empty;
    private Dictionary<string, Question> questionsDictionary = new Dictionary<string, Question>();
    public Dictionary<string, int?> questionAnsweredCorrectly = new Dictionary<string, int?>();

    public Question CurrentQuestion { get; private set; }

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

        // Update time if the question screen is active
        TimeCounter.Instance.UpdateTime();
    }

    public void ShowQuestionScreen(GameObject item)
    {
        currentItemID = item.name;

        if (questionsDictionary.ContainsKey(currentItemID))
        {
            var question = questionsDictionary[currentItemID];
            DisplayQuestion(question);
        }
        else
        {
            StartCoroutine(GetRequest($"http://localhost:6000/api/questions/hard-level?username={StaticData.Username}", item));
        }
    }

    public void HideQuestionScreen()
    {
        questionScreen.SetActive(false);
        isQuestionScreenActive = false;
        player.GetComponent<Player.Player>().enabled = true;
    }

    IEnumerator GetRequest(string uri, GameObject item)
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

                    questionsDictionary[question.QuestionId] = question;
                    CurrentQuestion = question;
                    digitKey = question.KeyDigit;
                    item.name = question.QuestionId; // Rename the item with the QuestionId
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
        player.GetComponent<Player.Player>().enabled = false;

        // Allow re-selection of answers
        FindObjectOfType<SubmitAnswer>().ResetAnswerButtons();
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
                answers[i].name = answer.QuestionAnswerId; // Set QuestionAnswerId as the button's name for reference
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

    public void MarkQuestionAsAnswered(string questionID, int? keyDigit)
    {
        questionAnsweredCorrectly[questionID] = keyDigit;
    }
}
