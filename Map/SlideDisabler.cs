using UnityEngine;

public class SlideDisabler : MonoBehaviour
{
    [SerializeField] private bool isLeftEdge = true;
    private EdgeCollider2D slide;

    private void Start()
    {
        slide = GetComponentInParent<EdgeCollider2D>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            slide.enabled = false;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (isLeftEdge && collision.gameObject.tag == "Player")
        {
            slide.enabled = true;
        }
    }
}
