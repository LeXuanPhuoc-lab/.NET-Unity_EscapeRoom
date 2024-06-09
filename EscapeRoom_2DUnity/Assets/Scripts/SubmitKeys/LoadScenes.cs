using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadScenes : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the collided object has the tag "NextRoom" and if the UI is not visible
        if (collision.CompareTag("NextRoom"))
        {
            // Log a message to the console
            Debug.Log("Unlock here");

            // Set the isTrigger property to false for the collided object's collider
            collision.isTrigger = false;

        }
    }
}
