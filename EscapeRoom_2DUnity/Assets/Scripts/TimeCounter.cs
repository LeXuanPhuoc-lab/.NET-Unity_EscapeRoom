using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeCounter : MonoBehaviour
{
    public static TimeCounter Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        UpdateTime();
    }
    
    public void UpdateTime()
    {
        // Debug.Log("Update Time");
        if (StaticData.RemainTime > 0)
        {
            StaticData.RemainTime = Math.Max(0, StaticData.RemainTime - Time.deltaTime);
        }
    }

    public static void TimePenalty(){
        StaticData.RemainTime = Math.Max(0, StaticData.RemainTime - 30);
    }
}