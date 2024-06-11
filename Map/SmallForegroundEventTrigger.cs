using UnityEngine;

public class SmallForegroundEventTrigger : MonoBehaviour
{
    public bool isActive = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (isActive)
            {
                ForegroundEventManager.Singleton.ActivateSmallForegroundEvent();
            }
        }
    }
}
