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

    private string existedQuestion = "Hello World";
    private string existedAnswerA = "#";
    private string existedAnswerB = "#";
    private string existedAnswerC = "#";
    private string existedAnswerD = "#";
    private string existedImageQuestion ="";

    void Start()
    {
        if (!existedQuestion.Equals("Hello World") &&
            !existedAnswerA.Equals("#") &&
            !existedAnswerB.Equals("#") &&
            !existedAnswerC.Equals("#") &&
            !existedAnswerD.Equals("#"))
        {
            answerA.GetComponentInChildren<TMP_Text>().text = existedAnswerA;
            answerB.GetComponentInChildren<TMP_Text>().text = existedAnswerB;
            answerC.GetComponentInChildren<TMP_Text>().text = existedAnswerC;
            answerD.GetComponentInChildren<TMP_Text>().text = existedAnswerD;
            ShowQuestion(existedQuestion);
            if (!string.IsNullOrEmpty(existedImageQuestion))
            {
                ShowImage();
            }

            return;
        }

        StartCoroutine(GetRequest("http://localhost:6000/api/questions/hard-level?username=test"));
    }

    // ReSharper disable Unity.PerformanceAnalysis
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
                    Debug.Log(question);
                    Debug.Log($"QuestionDesc: {question.QuestionDesc}");
                    Debug.Log($"Image: {question.Image}");
                    if (!string.IsNullOrWhiteSpace(question.Image))
                    {
                        StartCoroutine(LoadImage(question.Image));
                        existedImageQuestion = question.Image;
                    }
                    ShowQuestion(question.QuestionDesc);
                    SetAnswers(question.QuestionAnswers);
                }
            }
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
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
        existedQuestion = questionDesc;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    void SetAnswers(ICollection<QuestionAnswer> questionAnswers)
    {
        foreach (var answer in questionAnswers)
        {
            if (existedAnswerA.Equals("#"))
            {
                answerA.GetComponentInChildren<TMP_Text>().text = answer.Answer;
                existedAnswerA = answer.Answer;
            }
            else if (existedAnswerB.Equals("#"))
            {
                answerB.GetComponentInChildren<TMP_Text>().text = answer.Answer;
                existedAnswerB = answer.Answer;
            }
            else if (existedAnswerC.Equals("#"))
            {
                answerC.GetComponentInChildren<TMP_Text>().text = answer.Answer;
                existedAnswerC = answer.Answer;
            }
            else if (existedAnswerD.Equals("#"))
            {
                answerD.GetComponentInChildren<TMP_Text>().text = answer.Answer;
                existedAnswerD = answer.Answer;
            }

            Debug.Log($"Answer: {answer.Answer}, IsTrue: {answer.IsTrue}");
        }
    }
}
