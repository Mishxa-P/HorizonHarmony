using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] public Transform pointForDestruction;
    [SerializeField] private Animator animator;
    [SerializeField] MapGenerationManager.LocationState obstacleType;
    public void PlayDestroyAnimation()
    {
        animator.SetBool("IsDestroyed", true);
        switch (obstacleType)
        {
            case MapGenerationManager.LocationState.Desert:
                AudioManager.Singleton.Play("Rock_crash");
                break;
            case MapGenerationManager.LocationState.Ocean:
                AudioManager.Singleton.Play("Coral_crash");
                break;
            case MapGenerationManager.LocationState.SnowMountains:
                AudioManager.Singleton.Play("Rock_crash");
                AudioManager.Singleton.Play("Snowbrake");
                break;
        }
    }
}
