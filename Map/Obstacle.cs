using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] public Transform pointForDestruction;
    [SerializeField] private Animator animator;
    [SerializeField] MapGenerationManager.LocationState obstacleType;
    public void PlayDestroyAnimation()
    {
        animator.SetBool("IsDestroyed", true);
        AudioManager.Singleton.Play("Rock_crash");
        if (obstacleType == MapGenerationManager.LocationState.SnowMountains)
        {
            AudioManager.Singleton.Play("Snowbrake");
        }
    }
}
