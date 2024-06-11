using TMPro;
using UnityEngine;

public class Statistics : MonoBehaviour
{
    [SerializeField] private TMP_Text totalDistance;
    [SerializeField] private TMP_Text totalGameDuration;
    [SerializeField] private TMP_Text averageDistance;
    [SerializeField] private TMP_Text maxDistance;
    [SerializeField] private TMP_Text totalSlideDistance;
    [SerializeField] private TMP_Text totalTricksCount;
    [SerializeField] private TMP_Text totalObstaclesDestroyed;
    [SerializeField] private TMP_Text totalCoinsCollected;
    [SerializeField] private TMP_Text totalLocationChangedCount;
    [SerializeField] private TMP_Text totalDeathCount;

    public void UpdateStatistics()
    {
        totalDistance.text = GameData.totalDistance.ToString();
        if (GameData.totalGameDuration.TotalHours >= 10)
        {
            totalGameDuration.text = ((int)GameData.totalGameDuration.TotalHours).ToString() + ":" + GameData.totalGameDuration.ToString(@"mm\:ss");
        }
        else
        {
            totalGameDuration.text = "0" + ((int)GameData.totalGameDuration.TotalHours).ToString() + ":" + GameData.totalGameDuration.ToString(@"mm\:ss");
        }
       
        if (GameData.totalDeathCount > 0)
        {
            averageDistance.text = (GameData.totalDistance / GameData.totalDeathCount).ToString();
        }
        maxDistance.text = GameData.maxDistance.ToString();
        totalSlideDistance.text = GameData.totalSlideDistance.ToString();
        totalTricksCount.text = GameData.totalTricksCount.ToString();
        totalObstaclesDestroyed.text = GameData.totalObstaclesDestroyed.ToString();
        totalCoinsCollected.text = GameData.totalCoinsCollected.GetCoins().ToString();
        totalLocationChangedCount.text = GameData.totalLocationChangedCount.ToString();
        totalDeathCount.text = GameData.totalDeathCount.ToString();
    }
}
