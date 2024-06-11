using System.Collections;
using UnityEngine;
public class Wind : MonoBehaviour
{
    [Header("Position points")]
    [SerializeField] Transform leftPos;
    [SerializeField] Transform rightPos;
    [SerializeField] Transform bottomPos;
    [SerializeField] Transform topPos;

    [Space(10)]
    [Header("Time")]
    [SerializeField] float minSpawnInterval;
    [SerializeField] float maxSpawnInterval;
    [SerializeField] float minDuration;
    [SerializeField] float maxDuration;

    private SpriteRenderer windSprite;
    private BoxCollider2D windCollider;
    private bool canBeSpawned = true;
    private void Start()
    {
        windSprite = GetComponent<SpriteRenderer>();
        windSprite.enabled = false;
        windCollider = GetComponent<BoxCollider2D>();
        windCollider.enabled = false;
    }
    private void Update()
    {
        if (canBeSpawned)
        {
            StartCoroutine(SpawnWind(Random.Range(minDuration, maxDuration), Random.Range(minSpawnInterval, maxSpawnInterval)));
        }
    }

    private IEnumerator SpawnWind(float duration, float cooldown)
    {
        canBeSpawned = false;
        Activate();
        transform.position = new Vector3(Random.Range(leftPos.position.x, rightPos.position.x), 
            Random.Range(bottomPos.position.y, topPos.position.y), transform.localPosition.z);
        yield return new WaitForSeconds(duration);
        Deactivate();
        yield return new WaitForSeconds(cooldown);
        canBeSpawned = true;
    }

    private void Activate()
    {
        windSprite.enabled = true;
        windCollider.enabled = true;
    }

    private void Deactivate()
    {
        windSprite.enabled = false;
        windCollider.enabled = false;
    }
}
