using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticData : MonoBehaviour
{
    public static float RemainTime = 5 * 60;
    public static string Username = string.Empty;
    public static string CurrentScreen = "MH1";
    public static string KeyOder = string.Empty;
    public static string SessionId = String.Empty;
    public static string PlayerId = string.Empty;

    public static Dictionary<string, Call_Question_API.Question> QuestionsDictionary =
        new Dictionary<string, Call_Question_API.Question>();

    public static Dictionary<string, int?> QuestionAnsweredCorrectly = new Dictionary<string, int?>();
}
