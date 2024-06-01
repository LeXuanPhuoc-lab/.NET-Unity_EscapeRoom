using System;
using UnityEngine;

public class QuestionTrigger : MonoBehaviour
{
    public string itemID; // Thêm thuộc tính này
    private Call_Question_API questionAPI;
    private bool isPlayerNear = false;

    void Start()
    {
        questionAPI = FindObjectOfType<Call_Question_API>();
    }

    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E) && !Call_Question_API.isQuestionScreenActive)
        {
            questionAPI.ShowQuestionScreen(itemID); // Truyền itemID thay vì GameObject
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Hello world 1");
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("Bye bye :>");
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
        }
    }
}