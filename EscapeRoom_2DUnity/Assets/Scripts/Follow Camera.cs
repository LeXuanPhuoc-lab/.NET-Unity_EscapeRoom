using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] private GameObject followed;

    void Update()
    {
        transform.position = followed.transform.position + new Vector3(0, 0, -10);
    }
}