using System;
using Unity.VisualScripting;
using UnityEngine;

public class QuestionTrigger : MonoBehaviour
{
    private Call_Question_API questionAPI;
    private HintTrigger hintTrigger;
    private bool isPlayerNear = false;

    void Start()
    {
        // Find the Call_Question_API script in the scene
        questionAPI = FindObjectOfType<Call_Question_API>();
        // Find the HintTrigger script in the scene
        hintTrigger = FindObjectOfType<HintTrigger>();
    }

    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E) && !Call_Question_API.isQuestionScreenActive)
        {
            if (questionAPI.questionAnsweredCorrectly.ContainsKey(gameObject.name))
            {
                hintTrigger.ShowHint(questionAPI.questionAnsweredCorrectly[gameObject.name] ?? 0);
            }
            else
            {
                // Show question screen
                questionAPI.ShowQuestionScreen(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Hello World 1");
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
        }
    }
}