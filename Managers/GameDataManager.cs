using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    [Space(30)]
    [Header("Development")]
    [SerializeField] private float currentTimeScale = 1.0f;
    [SerializeField] private bool setTimeScale = false;

    private void OnEnable()
    {
        PlayerDeathZone.onPlayerDied += UpdateTotalResults;
    }
    private void OnDisable()
    {
        PlayerDeathZone.onPlayerDied -= UpdateTotalResults;
    }
    private void Start()
    {
        Time.timeScale = 1.0f;
        Timer.Begin();
        SetLocalDefaultValues();
        PlayerInputManager.Singleton.EnableInput();
    }
    private void Update()
    {
#if UNITY_EDITOR
        if (setTimeScale)
        {
            Time.timeScale = currentTimeScale;
        }
#endif
    }
    private void UpdateTotalResults()
    {
        GameData.currentTotalCoins.AddCoins(GameData.localCoins.GetCoins());
        GameData.totalCoinsCollected.AddCoins(GameData.localCoins.GetCoins());
        GameData.totalDistance += GameData.localDistance;
        if (GameData.maxDistance < GameData.localDistance)
        {
            GameData.maxDistance = GameData.localDistance;
        }
        GameData.totalDeathCount++;
        GameData.totalSlideDistance += GameData.localSlideDistance;
        GameData.totalTricksCount += GameData.localTricksCount;
        GameData.totalLocationChangedCount += GameData.localLocationChangedCount;

        Debug.Log("Total coins = " + GameData.currentTotalCoins.GetCoins());
        Debug.Log("Total distance = " + GameData.totalDistance);
        Debug.Log("Total deaths = " + GameData.totalDeathCount);
    }
    private void SetLocalDefaultValues()
    {
        GameData.localCoins = new Coin(0);
        GameData.localDistance = 0;
        GameData.localSlideDistance = 0;
        GameData.localTricksCount = 0;
        GameData.localLocationChangedCount = 0;
        GameData.localObstaclesDestroyed = 0;
    }
}
