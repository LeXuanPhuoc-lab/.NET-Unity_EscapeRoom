using System;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
public class QuestionTrigger : MonoBehaviour
{
    private Call_Question_API questionAPI;
    private HintTrigger hintTrigger;
    private bool isPlayerNear = false;
    public GameObject textGuide;
    AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("audio").GetComponent<AudioManager>();
    }

    void Start()
    {
        // Find the Call_Question_API script in the scene
        questionAPI = FindObjectOfType<Call_Question_API>();
        // Find the HintTrigger script in the scene
        hintTrigger = FindObjectOfType<HintTrigger>();

        textGuide.SetActive(false);
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
                // Open chest sound sfx
                audioManager.PlaySFX(audioManager.openChest);
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
            textGuide.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("Bye bye :>");
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            textGuide.SetActive(false);
        }
    }
}