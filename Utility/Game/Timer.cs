using System;
using UnityEngine;

public static class Timer
{
    private static bool isActive = false;
    public static void Begin()
    {
        GameData.localStartTime = DateTime.Now;
        GameData.localGameDuration = new TimeSpan(0, 0, 0, 0);
        Debug.Log("Begin: " + GameData.localGameDuration.ToString(@"hh\:mm\:ss"));
        isActive = true;
    }

    public static void Pause()
    {
        GameData.localGameDuration = GameData.localGameDuration.Add(DateTime.Now.Subtract(GameData.localStartTime));
        Debug.Log("Pause: " + GameData.localGameDuration.ToString(@"hh\:mm\:ss"));
    }

    public static void Unpause()
    {
        GameData.localStartTime = DateTime.Now;
        Debug.Log("Unpause: " + GameData.localGameDuration.ToString(@"hh\:mm\:ss"));
    }

    public static void End()
    {
        if (isActive)
        {
            GameData.localGameDuration = GameData.localGameDuration.Add(DateTime.Now.Subtract(GameData.localStartTime));
            Debug.Log("End: " + GameData.localGameDuration.ToString(@"hh\:mm\:ss"));
            GameData.totalGameDuration = GameData.totalGameDuration.Add(GameData.localGameDuration);
            isActive = false;
        }
    }
}
