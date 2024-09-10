using UnityEngine;

public class SlideDisabler : MonoBehaviour
{
    [SerializeField] private bool isLeftEdge = true;
    [SerializeField] private EdgeCollider2D slide;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("Disabling slide!");
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
