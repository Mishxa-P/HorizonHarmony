using System;
using UnityEngine;

public class PlayerCoinPicker : MonoBehaviour
{
    [SerializeField] private int coinValue = 1;

    public static Action onCoinsCollected;
    private void OnTriggerEnter2D(Collider2D coinCollider)
    {
        if (coinCollider.gameObject.tag == "Coin")
        {
            GameData.localCoins.AddCoins(coinValue);
            onCoinsCollected?.Invoke();
            coinCollider.GetComponent<Animator>().SetTrigger("PickedUp");
            AudioManager.Singleton.Play("Take_coin");
        }
    }
}
