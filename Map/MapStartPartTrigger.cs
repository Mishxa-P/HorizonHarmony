using UnityEngine;

public class MapStartPartTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {    
            MapGenerationManager.Singleton.SetLastPlayerReachedMapPart(GetComponentInParent<MapPart>().gameObject);
            MapGenerationManager.Singleton.SpawnMapPart(GetComponentInParent<MapPart>());
        }
    }
}
