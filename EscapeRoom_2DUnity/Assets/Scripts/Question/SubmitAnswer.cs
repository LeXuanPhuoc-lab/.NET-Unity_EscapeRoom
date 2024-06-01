using System.Collections;
using Models;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SubmitAnswer : MonoBehaviour
{
    public Button answerA;
    public Button answerB;
    public Button answerC;
    public Button answerD;
    public Call_Question_API callQuestionApi;
    public HintTrigger hintTrigger;

    void Start()
    {
        answerA.onClick.AddListener(() => Submit(answerA));
        answerB.onClick.AddListener(() => Submit(answerB));
        answerC.onClick.AddListener(() => Submit(answerC));
        answerD.onClick.AddListener(() => Submit(answerD));
    }

    void Submit(Button answerButton)
    {
        if (callQuestionApi.CurrentQuestion == null) return;

        string selectedAnswerId = answerButton.name;
        StartCoroutine(PostSubmitAnswer(callQuestionApi.CurrentQuestion.QuestionId, selectedAnswerId));
    }

    IEnumerator PostSubmitAnswer(string questionId, string selectAnswerId)
    {
        string uri = "http://localhost:6000/api/questions/submit-answer";
        var requestBody = new
        {
            username = "kingchen2",
            questionId = questionId,
            selectAnswerId = selectAnswerId
        };
        string jsonBody = JsonConvert.SerializeObject(requestBody);

        using (UnityWebRequest webRequest = new UnityWebRequest(uri, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                webRequest.result == UnityWebRequest.Result.DataProcessingError)
            {
                Debug.LogError("Error: " + webRequest.error);
            }
            else if (webRequest.result == UnityWebRequest.Result.Success)
            {
                var jsonResponse = webRequest.downloadHandler.text;
                var baseResponse = JsonConvert.DeserializeObject<BaseResponse<SubmitAnswerResponse>>(jsonResponse);
                if (baseResponse.StatusCode == 200)
                {
                    if (baseResponse.Data != null)
                    {
                        Debug.Log("Chúc mừng bạn, đáp án chính xác");
                        callQuestionApi.MarkQuestionAsAnswered(callQuestionApi.CurrentQuestion.QuestionId, baseResponse.Data.KeyDigit);
                        callQuestionApi.HideQuestionScreen();
                        hintTrigger.ShowHint(baseResponse.Data.KeyDigit);
                    }
                    else
                    {
                        Debug.Log("Đáp án không chính xác");
                        TimeCounter.TimePenalty();
                    }
                }
                else
                {
                    Debug.LogError("Failed to submit answer");
                }
            }
        }
        
    }
    public void ResetAnswerButtons()
    {
        Button[] answerButtons = { answerA, answerB, answerC, answerD };
        foreach (var button in answerButtons)
        {
            button.interactable = true;
        }
    }
    private class SubmitAnswerResponse
    {
        public int KeyDigit { get; set; }
    }
}
