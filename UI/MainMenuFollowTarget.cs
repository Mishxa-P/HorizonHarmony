using UnityEngine;

public class MainMenuFollowTarget : MonoBehaviour
{
    [SerializeField] private float speed;
    private void FixedUpdate()
    {
        transform.position += new Vector3(speed * Time.fixedDeltaTime, 0.0f, 0.0f);
    }
}
