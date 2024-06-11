using UnityEngine;
public class Parallax : MonoBehaviour
{
    [SerializeField] public Transform followingTarget;

    [Range(0.0f, 1.0f)]
    [SerializeField] private float parallaxStrength = 0.1f;
    [SerializeField] public Vector3 offset;
    [SerializeField] private bool eventType = false;
    [SerializeField] private bool eventAddSpeed = false;
    [SerializeField] private float eventAdditionalSpeed = 25.0f;

    private Vector3 startPosition;
    private float length;

    private void Start()
    {
        if (!followingTarget)
        {
            followingTarget = Camera.main.transform;
        }
  
        startPosition = transform.position;
        if (gameObject.GetComponent<SpriteRenderer>() != null)
        {
            length = gameObject.GetComponent<SpriteRenderer>().bounds.size.x;
        }
        else
        {
            length = gameObject.GetComponentInChildren<SpriteRenderer>().bounds.size.x;
        }
    }
    [ContextMenu("ActivateEvent")]
    public void ActivateBackgroundEvent()
    {
        if (!followingTarget)
        {
            followingTarget = Camera.main.transform;
        }
        transform.position = new Vector3(followingTarget.position.x, followingTarget.position.y, 0.0f) + offset;
        startPosition = transform.position;
    }

    private void LateUpdate()
    {
        if (!eventType)
        {
            float temp = followingTarget.position.x * (1.0f - parallaxStrength);
            Vector3 distance = new Vector3(followingTarget.transform.position.x * parallaxStrength, followingTarget.transform.position.y - startPosition.y, 0);

            transform.position = startPosition + distance + offset;

            if (temp > startPosition.x + length && parallaxStrength < 0.999f)
            {
                startPosition.x += length;
            }
            else if (temp < startPosition.x - length && parallaxStrength < 0.999f)
            {
                startPosition.x -= length;
            }
        }
        else
        {
            Vector3 distance = new Vector3((followingTarget.position.x - startPosition.x) * parallaxStrength, followingTarget.position.y - startPosition.y, 0);
            if (eventAddSpeed)
            {
                offset -= new Vector3(eventAdditionalSpeed * Time.deltaTime, 0.0f, 0.0f);
            }
            transform.position = startPosition + distance + offset;
        }
    } 
}
