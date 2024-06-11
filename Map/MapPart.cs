using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
public class MapPart : MonoBehaviour
{
    [Serializable]
    private class Transforms
    {
        public int GetLength()
        {
            return transforms.Count;
        }
        public Transform GetTransform(int index)
        {
            return transforms[index];
        }

        [SerializeField] private List<Transform> transforms;
    }

    [SerializeField] private GameObject obstaclePrefab;
    [SerializeField] private List<Transform> obstaclePossibleLocations;
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private List<Transforms> coinGroupsPossibleLocations;
    [SerializeField] private GameObject slide;
    [SerializeField] private GameObject city;
    [SerializeField] private GameObject animalPack;
    [SerializeField] private PartType partType;

    public PartType Type { get => partType; private set => partType = value; }

    public enum PartType
    {
        Flat, High
    }

    [HideInInspector]
    public GameObject nextPart;

    private void Start()
    {
        SpawnObstacles();
        SpawnCoinGroups();
        SpawnSlide();
        SpawnCity();
        SpawnAnimals();
    }

    private void OnEnable()
    {
        MapGenerationManager.onDestroyCurrentLocationNeedlessParts += DestroyLocation;
    }
    private void OnDisable()
    {
        MapGenerationManager.onDestroyCurrentLocationNeedlessParts -= DestroyLocation;
    }

    private void DestroyLocation(float x)
    {
        if (GetComponentInChildren<SpriteRenderer>().bounds.center.x > x)
        {
            Destroy(gameObject);
        }
    }

    private void SpawnObstacles()
    {
        if (GameSettings.gameMode == ModesMenu.GameMode.Relax)
        {
            return;
        }
        int num = Random.Range(0, obstaclePossibleLocations.Count + 1);
        for (int i = 0; i < num; i++)
        {
            int index = Random.Range(0, obstaclePossibleLocations.Count);
            GameObject obj = Instantiate(obstaclePrefab, obstaclePossibleLocations[index].parent);
            obj.transform.localPosition = obstaclePossibleLocations[index].localPosition;
            obj.transform.localRotation = obstaclePossibleLocations[index].localRotation;
            obj.transform.localScale = obstaclePossibleLocations[index].localScale;
            obstaclePossibleLocations.Remove(obstaclePossibleLocations[index]);
        }
    }
    private void SpawnCoinGroups()
    {
        if (GameSettings.gameMode == ModesMenu.GameMode.Relax)
        {
            return;
        }
        int numOfGroups = Random.Range(0, coinGroupsPossibleLocations.Count + 1);
        for (int i = 0; i < numOfGroups; i++)
        {
            int index = Random.Range(0, coinGroupsPossibleLocations.Count);
            int numOfCoins = coinGroupsPossibleLocations[index].GetLength();
            for (int j = 0; j < numOfCoins; j++) 
            {
                GameObject obj = Instantiate(coinPrefab, coinGroupsPossibleLocations[index].GetTransform(0).parent);
                obj.transform.localPosition = coinGroupsPossibleLocations[index].GetTransform(j).localPosition;
                obj.transform.localRotation = coinGroupsPossibleLocations[index].GetTransform(j).localRotation;
                obj.transform.localScale = coinGroupsPossibleLocations[index].GetTransform(j).localScale;
            }
            coinGroupsPossibleLocations.Remove(coinGroupsPossibleLocations[index]);
        }
    }  
    private void SpawnSlide()
    {
        if(slide != null)
        {
            int num = Random.Range(0, 2);
            if (num == 0)
            {
                slide.SetActive(false);
            }
            else
            {
                slide.SetActive(true);
            }
        }
    }
    private void SpawnCity()
    {
        if (city != null)
        {
            int num = Random.Range(0, 2);
            if (num == 0)
            {
                city.SetActive(false);
            }
            else
            {
                city.SetActive(true);
            }
        }
    }
    private void SpawnAnimals()
    {
        if (animalPack != null)
        {
            int num = Random.Range(0, 2);
            if (num == 0)
            {
                animalPack.SetActive(false);
            }
            else
            {
                animalPack.SetActive(true);
            }
        }
    }
    public Vector3 GetSpriteRendererCenterOffset()
    {
        return GetComponentInChildren<SpriteRenderer>().bounds.center - transform.position;
    }

    public Bounds GetSpriteRendererBounds()
    {
        return GetComponentInChildren<SpriteRenderer>().bounds;
    }

    public void ChangeSprite(Sprite mapPartSprite)
    {
        GetComponent<SpriteRenderer>().sprite = mapPartSprite;
    }
}
