using TMPro;
using UnityEngine;

public class Result : MonoBehaviour
{
    [SerializeField] private TMP_Text timeResult;
    [SerializeField] private TMP_Text distanceResult;
    [SerializeField] private TMP_Text coinsResult;

    public void UpdateResult()
    {
        timeResult.text = GameData.localGameDuration.ToString(@"hh\:mm\:ss");
        distanceResult.text = GameData.localDistance.ToString();
        coinsResult.text = GameData.localCoins.GetCoins().ToString();
    }
}
