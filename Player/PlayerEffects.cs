using System.Collections;
using UnityEngine;

public class PlayerEffects : MonoBehaviour
{
    [SerializeField] private Animator invulnerabilityEffectAnimator;
    [SerializeField] private Animator accelerationEffectAnimator;
    [SerializeField] private Animator landingEffectAnimator;
    public void PlayLandingEffect()
    {
        EnableLandingEffect();
        switch (MapGenerationManager.Singleton.CurrentLocationState)
        {
            case MapGenerationManager.LocationState.Desert:
                landingEffectAnimator.SetTrigger("DesertLanding");
                StartCoroutine(ResetTrigger(landingEffectAnimator, "DesertLanding"));
                break;
            case MapGenerationManager.LocationState.Ocean:
                landingEffectAnimator.SetTrigger("OceanLanding");
                StartCoroutine(ResetTrigger(landingEffectAnimator, "OceanLanding"));
                break;
            case MapGenerationManager.LocationState.SnowMountains:
                landingEffectAnimator.SetTrigger("SnowLanding");
                StartCoroutine(ResetTrigger(landingEffectAnimator, "SnowLanding"));
                break;
            default:
                break;
        }
    }
    public void PlayInvulnerabilityEffect(float duration)
    {
        invulnerabilityEffectAnimator.SetBool("Invulnerability", true);
        StartCoroutine(SetBoolAfterDelay(invulnerabilityEffectAnimator, "Invulnerability", duration));
    }
    public void PlayAccelerationEffect(float duration)
    {
        accelerationEffectAnimator.SetBool("Acceleration", true);
        StartCoroutine(SetBoolAfterDelay(accelerationEffectAnimator, "Acceleration", duration));
    }
    public void DisableLandingEffect()
    {
        landingEffectAnimator.GetComponent<SpriteRenderer>().enabled = false;
    }
    private void EnableLandingEffect()
    {
        landingEffectAnimator.GetComponent<SpriteRenderer>().enabled = true;
    }
    private IEnumerator ResetTrigger(Animator animator, string triggerName, float time = 0.25f)
    {
        yield return new WaitForSeconds(time);
        animator.ResetTrigger(triggerName);
    }
    private IEnumerator SetBoolAfterDelay(Animator animator, string boolName, float time = 0.25f, bool value = false)
    {
        yield return new WaitForSeconds(time);
        animator.SetBool(boolName, value);
    }
}
