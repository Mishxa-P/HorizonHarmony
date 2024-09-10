using System.Collections;
using UnityEngine;

public class BigForegroundEventTrigger : MonoBehaviour
{
    public bool isActive = false;

    private bool destroyable = true;
    private void OnEnable()
    {
        PlayerDeathZone.onPlayerDied += ChangeToUndestroyable;
    }
    private void OnDisable()
    {
        PlayerDeathZone.onPlayerDied -= ChangeToUndestroyable;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (isActive)
            {
                ForegroundEventManager.Singleton.ActivateBigForegroundEvent();
            }
            StartCoroutine(Destroy(MapGenerationManager.Singleton.DestroyTime));
        }
    }
    private IEnumerator Destroy(float time)
    {
        yield return new WaitForSeconds(time);
        if (this != null && destroyable)
        {
            Destroy(GetComponentInParent<MapPart>().gameObject);
        }
    }
    private void ChangeToUndestroyable()
    {
        destroyable = false;
    }
}
