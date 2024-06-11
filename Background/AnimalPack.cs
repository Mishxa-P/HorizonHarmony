using UnityEngine;

public class AnimalPack : MonoBehaviour
{
    [SerializeField] private AnimalType type;
    public enum AnimalType
    {
        Bird, Fish
    }
    [Header("Bird")]
    [SerializeField] private float endBirdDY = 15.0f;
    [SerializeField] private float birdAnimDuration = 3.0f;
    [SerializeField] private float birdSpawnDX = 1.0f;
    [Header("Fish")]
    [SerializeField] private float fishAnimDuration = 1.6f;
    [SerializeField] private float endFishDY = -9.0f;
    [SerializeField] private float fishSpawnDX = 1.0f;
    [SerializeField] private float fishSpawnDY = 0.5f;

    private AnimalAnimation[] animalAnimations;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            PlayAnimalsAnimation();
        }
    }
    private void Start()
    {
        animalAnimations = GetComponentsInChildren<AnimalAnimation>();
        switch (type)
        {
            case AnimalType.Bird:
                foreach (AnimalAnimation anim in animalAnimations)
                {
                    anim.gameObject.transform.position += new Vector3(Random.Range(birdSpawnDX, birdSpawnDX), 0.0f, 0.0f);
                    int rnd = Random.Range(0, 2);
                    Vector3 newScale = new Vector3(1.0f, anim.gameObject.transform.localScale.y, anim.gameObject.transform.localScale.z);
                    if (rnd == 1)
                    {
                        newScale = new Vector3(-1.0f, anim.gameObject.transform.localScale.y, anim.gameObject.transform.localScale.z);
                    }
                    anim.gameObject.transform.localScale = newScale;
                    anim.Initialize(AnimalType.Bird);
                }
                break;
            case AnimalType.Fish:
                foreach (AnimalAnimation anim in animalAnimations)
                {
                    anim.gameObject.transform.position += new Vector3(Random.Range(fishSpawnDX, fishSpawnDX), Random.Range(fishSpawnDY, fishSpawnDY), 0.0f);
                    anim.Initialize(AnimalType.Fish);                                               
                }                                                                        
                break;                                                                   
        }
    }
    public void PlayAnimalsAnimation()
    {
        switch (type)
        {
            case AnimalType.Bird:
                foreach (AnimalAnimation anim in animalAnimations)
                {
                    anim.GetComponent<AnimalAnimation>().PlayAnimation(AnimalType.Bird, anim.gameObject.transform.position + new Vector3(Random.Range(-5f, 5f), endBirdDY, 0.0f), birdAnimDuration);
                }
                break;
            case AnimalType.Fish:
                foreach (AnimalAnimation anim in animalAnimations)
                {
                    anim.GetComponent<AnimalAnimation>().PlayAnimation(AnimalType.Fish, anim.gameObject.transform.position + new Vector3(Random.Range(-5f, 5f), endFishDY, 0.0f), fishAnimDuration);
                }
                break;
        }
    }
}
