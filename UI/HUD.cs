using System.Collections;
using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour
{
    [SerializeField] private TMP_Text localCoins;
    [SerializeField] private TMP_Text localDistance;
    [SerializeField] private float delayBetweenDistanceUpdates = 0.75f;

    private bool canUpdateLocalDistance = true;
    private void OnEnable()
    {
        PlayerCoinPicker.onCoinsCollected += UpdateLocalCoins;
    }
    private void OnDisable()
    {
        PlayerCoinPicker.onCoinsCollected -= UpdateLocalCoins;
    }
    private void Update()
    {
        if (canUpdateLocalDistance)
        {
            UpdateLocalDistance();
            StartCoroutine(Delay());
        }
    }
    public void UpdateLocalCoins()
    {
        Debug.Log("Local coins = " + GameData.localCoins.GetCoins());
        localCoins.text = GameData.localCoins.GetCoins().ToString();
    }
    private void UpdateLocalDistance()
    {
        localDistance.text = GameData.localDistance.ToString();
    }
    private IEnumerator Delay()
    {
        canUpdateLocalDistance = false;
        yield return new WaitForSeconds(delayBetweenDistanceUpdates);
        canUpdateLocalDistance = true;
    }
}
