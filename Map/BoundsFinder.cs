using UnityEngine;

public class BoundsFinder : MonoBehaviour
{
    [SerializeField] private Transform topLeft;
    [SerializeField] private Transform bottomRight;
    public Bounds FindBounds()
    {
        Bounds result = new Bounds();
        result.center = (topLeft.position + bottomRight.position) / 2.0f;
        result.extents = new Vector3(bottomRight.position.x - topLeft.position.x, topLeft.position.y - bottomRight.position.y, 0.0f) / 2.0f;
        return result;
    }
}
