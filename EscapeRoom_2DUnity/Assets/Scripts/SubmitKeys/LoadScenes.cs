using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScenes : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the collided object has the tag "GoToNhaRoom"
        if (collision.CompareTag("GoToNhaRoom"))
        {
            SceneManager.LoadScene("RF Castle/Scenes/MH2");
        }if (collision.CompareTag("NextRoom"))
        {
            SceneManager.LoadScene("RF Castle/Scenes/Quang");
        }
        else
        {
            Debug.Log("cannot find the tag of the objects");
        }
    }
}