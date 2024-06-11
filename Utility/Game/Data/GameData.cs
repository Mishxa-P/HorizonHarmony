using System;

public static class GameData
{
    public static Coin localCoins;
    public static DateTime localStartTime;
    public static TimeSpan localGameDuration;
    public static uint localDistance;
    public static uint localSlideDistance;
    public static uint localLocationChangedCount;
    public static uint localTricksCount;
    public static uint localObstaclesDestroyed;

    public static Coin currentTotalCoins;
    public static Coin totalCoinsCollected;
    public static TimeSpan totalGameDuration;
    public static uint maxDistance;
    public static uint totalDistance;
    public static uint totalSlideDistance;
    public static uint totalDeathCount;
    public static uint totalLocationChangedCount;
    public static uint totalTricksCount;
    public static uint totalObstaclesDestroyed;

    public static DayTime.State dayTimeState;
    public static float dayTimeStartTime;
}
