using DG.Tweening;
using UnityEngine;
using System.Collections;

public class AnimalAnimation : MonoBehaviour
{
    //for fish anim
    private float moveDuration = 0.15f;  
    private float rotationAngle = 5f;
    private IEnumerator fishInfiniteRotationCoroutine;
    private IEnumerator fishIdleAnimationCoroutine;

    public void Initialize(AnimalPack.AnimalType type)
    {
        switch (type)
        {
            case AnimalPack.AnimalType.Bird:
                break;
            case AnimalPack.AnimalType.Fish:
                fishInfiniteRotationCoroutine = FishInfiniteRotation();
                StartCoroutine(fishInfiniteRotationCoroutine);
                fishIdleAnimationCoroutine = FishIdleAnimation();
                StartCoroutine(fishIdleAnimationCoroutine);
                break;
        }
    }
    public void PlayAnimation(AnimalPack.AnimalType type, Vector3 endPoint, float duration)
    {
        switch (type)
        {
            case AnimalPack.AnimalType.Bird:
                StartCoroutine(BirdFlyAwayAnimation(endPoint, duration));
                break;
            case AnimalPack.AnimalType.Fish:
                StopCoroutine(fishInfiniteRotationCoroutine);
                StopCoroutine(fishIdleAnimationCoroutine);
                StartCoroutine(FishSwimAwayAnimation(endPoint, duration));
                break;
        }
    }
    private IEnumerator FishInfiniteRotation(float delay = 0.0f)
    {
        yield return new WaitForSeconds(delay);
        while (true)
        {
            rotationAngle *= -1.0f;
            Quaternion newRotation = Quaternion.Euler(0, 0, rotationAngle) * transform.rotation;
            Tween rotateTween = transform.DOLocalRotateQuaternion(newRotation, moveDuration).SetEase(Ease.Linear)
                .SetLink(gameObject);
            yield return rotateTween.WaitForCompletion();
        }
    }
    private IEnumerator FishIdleAnimation()
    {
        while (true)
        {
            float dx = Random.Range(3.0f, 6.0f);
            Tween moveTween = transform.DOLocalMove(transform.localPosition + new Vector3(dx, 0.0f, 0.0f), Random.Range(4.0f, 6.0f)).SetEase(Ease.Linear);
            yield return moveTween.WaitForCompletion();
            moveTween = transform.DOScaleX(1.0f, 0.15f);
            yield return moveTween.WaitForCompletion();
            moveTween = transform.DOLocalMove(transform.localPosition - new Vector3(dx, 0.0f, 0.0f), Random.Range(4.0f, 6.0f)).SetEase(Ease.Linear);
            yield return moveTween.WaitForCompletion();
            moveTween = transform.DOScaleX(-1.0f, 0.15f);
            yield return moveTween.WaitForCompletion();
        }
    }
    private IEnumerator FishSwimAwayAnimation(Vector3 endPoint, float duration)
    {
        float angle = -Vector2.Angle(Vector2.right, transform.position - endPoint);
        if (transform.localScale.x == 1)
        {
            angle = 180.0f + angle;
        }
        Vector3 newRotation = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, angle); 
        float distance = Vector3.Distance(endPoint, transform.position);
        float delay = 0.15f;
        StartCoroutine(FishInfiniteRotation(delay + 0.05f));
        transform.DORotate(newRotation, delay);
        yield return new WaitForSeconds(delay);
        if (transform.localScale.x == 1)
        {
           distance *= -1;
        }
        transform.DOMove(transform.position + transform.right * distance, duration)
            .SetLink(gameObject);
    }
    private IEnumerator BirdFlyAwayAnimation(Vector3 endPoint, float duration)
    {
        GetComponent<Animator>().SetTrigger("Fly");
        if (transform.position.x > endPoint.x && transform.localScale.x == 1)
        {
            Tween rotateTween = transform.DOScaleX(-1.0f, 0.15f);
            yield return rotateTween.WaitForCompletion();
        }
        if (transform.position.x < endPoint.x && transform.localScale.x == -1)
        {
            Tween rotateTween = transform.DOScaleX(1.0f, 0.15f);
            yield return rotateTween.WaitForCompletion();
        }
        transform.DOMove(endPoint, duration)
            .SetLink(gameObject);
    }
    private void OnDestroy()
    {
        DOTween.Kill(gameObject);
    }
}
