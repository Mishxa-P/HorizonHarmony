using System;
using UnityEngine;

public class PlayerDeathZone : MonoBehaviour
{
    public static Action onPlayerDied;

    private bool playerIsDead = false;
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            if (!playerIsDead)
            {
                onPlayerDied?.Invoke();
                playerIsDead = true;
            }    
        }
    }
}
