using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapGenerationManager : MonoBehaviour
{
    [Header("MapParts")]
    [SerializeField] private List<GameObject> desertMatchMapParts;
    [SerializeField] private List<GameObject> oceanMatchMapParts;
    [SerializeField] private List<GameObject> snowMountainsMatchMapParts;
    [SerializeField] private int maxFlatMapPartsInARow = 2;
    [SerializeField] private int maxHighMapPartsInARow = 1;

    [Space(10)]
    [Header("TempMapPart")]
    [SerializeField] private GameObject tempMapPart;
    [SerializeField] private Sprite desertTempMapPartSprite;
    [SerializeField] private Sprite oceanTempMapPartSprite;
    [SerializeField] private Sprite snowMountainsTempMapPartSprite;

    [Header("Background")]
    [SerializeField] private GameObject desertBackground;
    [SerializeField] private GameObject oceanBackground;
    [SerializeField] private GameObject snowMountainsBackground;

    [Space(15)]
    [Header("Settings")]
    [SerializeField] private float destroyTime;
    public float DestroyTime { get { return destroyTime; } }

    [Space(15)]
    [Header("Development")]
    [SerializeField] bool setInitialState = false;
    [SerializeField] LocationState state;

    public static MapGenerationManager Singleton { get; private set; }

    private Vector2 nextPos;

    private int flatMapPartsInARow; // example: if 1 is flat and 2 if high THAN: flatMapPartsInARow = 0, highMapPartsInARow = 1
    private int highMapPartsInARow;
    private List<int> currentFlatMapPartsIndexes = new List<int>();
    private List<int> currentHighMapPartsIndexes = new List<int>();

    public static Action<float> onDestroyCurrentLocationNeedlessParts;
    public static Action<LocationState> onPlayerEquipmentChanged;

    private GameObject lastPlayerReachedPart;
    private GameObject lastSpawnedPart;
    private List<GameObject> lastTempMapParts = new List<GameObject>();

    private bool firstChange = true;

    public enum LocationState
    {
        Desert = 0,
        Ocean,
        SnowMountains
    }
    public LocationState CurrentLocationState { get; private set; }
    public LocationState NextLocationState { get; private set; }
 
    private void Awake()
    {
        if (Singleton != null) 
        {
            Debug.LogError("Map generation is already exist!"); 
        } 
        else 
        { 
            Singleton = this; 
        }
    }
    private void Start()
    {
        nextPos = transform.position;
        if (GameSettings.randomInitialLocState)
        {
            NextLocationState = (LocationState)Random.Range(0, 3);
        }
        else
        {
            NextLocationState = GameSettings.initialLocState;
        }
        if (setInitialState)
        {
            NextLocationState = state;
        }
        ChangeLocation();
        UpdateMapPartsIndexes();
        SpawnStartParts();
    }
    public Vector3 SpawnNewLocation()
    {
        if (GameSettings.locationMode == ModesMenu.LocationMode.OneLocation)
        {
            return lastPlayerReachedPart.GetComponent<MapPart>().nextPart.transform.GetChild(0).position;
        }

        onDestroyCurrentLocationNeedlessParts?.Invoke(lastPlayerReachedPart.GetComponent<MapPart>().nextPart.transform.position.x);

        int newLocationState = Random.Range(0, 3);
        while ((int)NextLocationState == newLocationState)
        {
            newLocationState = Random.Range(0, 3);
        }
        NextLocationState = (LocationState)newLocationState;

        GameObject lastCurrentLocationPart = lastPlayerReachedPart.GetComponent<MapPart>().nextPart;
        lastSpawnedPart = lastCurrentLocationPart;

        UpdateMapPartsIndexes();
        lastTempMapParts.Clear();

        switch (NextLocationState)
        {
            case LocationState.Desert:
                Spawn(desertMatchMapParts, default, true);
                Spawn(desertMatchMapParts, default, true);
                SpawnMapPart(lastSpawnedPart.GetComponent<MapPart>());
                break;
            case LocationState.Ocean:
                Spawn(oceanMatchMapParts, default, true);
                Spawn(desertMatchMapParts, default, true);
                SpawnMapPart(lastSpawnedPart.GetComponent<MapPart>());
                break;
            case LocationState.SnowMountains:
                Spawn(snowMountainsMatchMapParts, default, true);
                Spawn(desertMatchMapParts, default, true);
                SpawnMapPart(lastSpawnedPart.GetComponent<MapPart>());
                break;
            default:
                break;
        }
        lastTempMapParts[0].GetComponentInChildren<SmallForegroundEventTrigger>().isActive = true;
        lastTempMapParts[0].GetComponentInChildren<BigForegroundEventTrigger>().isActive = true;

        Debug.Log("New location is spawned");
        return lastSpawnedPart.transform.GetChild(0).position;
    }

    public void ChangeLocation()
    {
        if (CurrentLocationState == NextLocationState)
        {
            if (!firstChange)
            {
                return;
            }
        }
        if (!firstChange && GameSettings.locationMode == ModesMenu.LocationMode.OneLocation)
        {
            return;
        }
        Debug.Log("The location has changed");
        CurrentLocationState = NextLocationState;

        if (lastTempMapParts.Count > 0)
        {
            foreach (var tempMapPart in lastTempMapParts)
            {
                SpriteRenderer spriteRenderer = tempMapPart.GetComponentInChildren<SpriteRenderer>();
                switch (CurrentLocationState)
                {
                    case LocationState.Desert:
                        spriteRenderer.sprite = desertTempMapPartSprite;
                        break;
                    case LocationState.Ocean:
                        spriteRenderer.sprite = oceanTempMapPartSprite;
                        break;
                    case LocationState.SnowMountains:
                        spriteRenderer.sprite = snowMountainsTempMapPartSprite;
                        break;
                }
            }
           
        }

        onPlayerEquipmentChanged?.Invoke(NextLocationState);
        ChangeMusic();
        ChangeBackground();
        BackgroundEventManager.Singleton.DeactivateAllEvents();
        if (firstChange)
        {
            firstChange = false;
        }
        else
        {
            GameData.totalLocationChangedCount++;
        }
    }
    public void SetLastPlayerReachedMapPart(GameObject lastPlayerReachedPart)
    {
        this.lastPlayerReachedPart = lastPlayerReachedPart;
    }
    public void SpawnMapPart(MapPart lastPart, bool firstPart = false)
    {
        if (lastPart.nextPart != null)
        {
            return;
        }

        int rnd = Random.Range(0, 2);

        MapPart.PartType partType = MapPart.PartType.Flat;

        // first map part is always HIGH
        if (firstPart)
        {
            highMapPartsInARow++;
            partType = MapPart.PartType.High;
        }
        else
        {
            if (rnd == 0)
            {
                if (flatMapPartsInARow < maxFlatMapPartsInARow)
                {
                    flatMapPartsInARow++;
                    highMapPartsInARow = 0;
                    partType = MapPart.PartType.Flat;
                }
                else
                {
                    highMapPartsInARow++;
                    flatMapPartsInARow = 0;
                    partType = MapPart.PartType.High;
                }
            }
            else
            {
                if (highMapPartsInARow < maxHighMapPartsInARow)
                {
                    highMapPartsInARow++;
                    flatMapPartsInARow = 0;
                    partType = MapPart.PartType.High;
                }
                else
                {
                    flatMapPartsInARow++;
                    highMapPartsInARow = 0;
                    partType = MapPart.PartType.Flat;
                }
            }
        }
       
        switch (NextLocationState)
        {
            case LocationState.Desert:
                Spawn(desertMatchMapParts, partType);
                break;
            case LocationState.Ocean:
                Spawn(oceanMatchMapParts, partType);
                break;
            case LocationState.SnowMountains:
                Spawn(snowMountainsMatchMapParts, partType);
                break;
            default:
                break;
        }
    }

    private void Spawn(List<GameObject> currentLocationMapPartsList, MapPart.PartType partType = MapPart.PartType.Flat, bool isTempMapPart = false)
    {
        GameObject mapPart = null;
        SpriteRenderer spriteRenderer = tempMapPart.GetComponentInChildren<SpriteRenderer>();
 
        if (isTempMapPart)
        {
            switch (CurrentLocationState)
            {
                case LocationState.Desert:
                    spriteRenderer.sprite = desertTempMapPartSprite;
                    break;
                case LocationState.Ocean:
                    spriteRenderer.sprite = oceanTempMapPartSprite;
                    break;
                case LocationState.SnowMountains:
                    spriteRenderer.sprite = snowMountainsTempMapPartSprite;
                    break;
                default:
                    return;
            }
            mapPart = tempMapPart;
        }
        else
        {
            int index = 0;
            switch (partType)
            {
                case MapPart.PartType.Flat:
                    index = Random.Range(0, currentFlatMapPartsIndexes.Count);
                    index = currentFlatMapPartsIndexes[index];
                    break;
                case MapPart.PartType.High:
                    index = Random.Range(0, currentHighMapPartsIndexes.Count);
                    index = currentHighMapPartsIndexes[index];
                    break;
            }
            mapPart = currentLocationMapPartsList[index];
        }

        if (lastSpawnedPart == null)
        {
            GameObject firstMapPart = Instantiate(mapPart, nextPos, Quaternion.identity);
            lastSpawnedPart = firstMapPart;
            return;
        }

        MapPart newPart = mapPart.GetComponent<MapPart>();
        nextPos.x = lastSpawnedPart.transform.GetChild(0).position.x - newPart.GetSpriteRendererCenterOffset().x + newPart.GetSpriteRendererBounds().extents.x;
        nextPos.y = lastSpawnedPart.transform.GetChild(0).position.y - newPart.GetSpriteRendererCenterOffset().y - newPart.GetSpriteRendererBounds().extents.y;
        GameObject newMapPart = Instantiate(mapPart, nextPos, Quaternion.identity);
        lastSpawnedPart.GetComponent<MapPart>().nextPart = newMapPart;
        lastSpawnedPart = newMapPart;

        if (isTempMapPart)
        {
            lastTempMapParts.Add(newMapPart);
        }
    }
    private void SpawnStartParts()
    {
        SpriteRenderer spriteRenderer = tempMapPart.GetComponentInChildren<SpriteRenderer>();
        switch (CurrentLocationState)
        {
            case LocationState.Desert:
                spriteRenderer.sprite = desertTempMapPartSprite;
                break;
            case LocationState.Ocean:
                spriteRenderer.sprite = oceanTempMapPartSprite;
                break;
            case LocationState.SnowMountains:
                spriteRenderer.sprite = snowMountainsTempMapPartSprite;
                break;
            default:
                return;
        }

        GameObject firstMapPart = Instantiate(tempMapPart, nextPos, Quaternion.identity);
        lastSpawnedPart = firstMapPart;

        SpawnMapPart(lastSpawnedPart.GetComponent<MapPart>(), true);
    }

    private void UpdateMapPartsIndexes()
    {
        List<GameObject> mapParts = new List<GameObject>();
        switch (NextLocationState)
        {
            case LocationState.Desert:
                mapParts = desertMatchMapParts;
                break;
            case LocationState.Ocean:
                mapParts = oceanMatchMapParts;
                break;
            case LocationState.SnowMountains:
                mapParts = snowMountainsMatchMapParts;
                break;
        }
        UpdateMapPartsForLocation(mapParts);
    }
    private void UpdateMapPartsForLocation(List<GameObject> currentLocationMapParts)
    {
        currentFlatMapPartsIndexes.Clear();
        currentHighMapPartsIndexes.Clear();
        for (int i = 0; i < currentLocationMapParts.Count; i++)
        {
            if (currentLocationMapParts[i].GetComponent<MapPart>().Type == MapPart.PartType.Flat)
            {
                currentFlatMapPartsIndexes.Add(i);
            }
            else
            {
                currentHighMapPartsIndexes.Add(i);
            }
        }
    }
    private void ChangeMusic()
    {
        switch (NextLocationState)
        {
            case LocationState.Desert:
                AudioManager.Singleton.FadeIn("DesertMusic", 5.0f);
                AudioManager.Singleton.FadeOut("OceanMusic", 3.0f);
                AudioManager.Singleton.FadeOut("SnowMountainsMusic", 3.0f);
                AudioManager.Singleton.Stop("Waves");
                break;
            case LocationState.Ocean:
                AudioManager.Singleton.FadeOut("DesertMusic", 3.0f);
                AudioManager.Singleton.FadeIn("OceanMusic", 5.0f);
                AudioManager.Singleton.FadeOut("SnowMountainsMusic", 3.0f);
                AudioManager.Singleton.Play("Waves");
                break;
            case LocationState.SnowMountains:
                AudioManager.Singleton.FadeOut("DesertMusic", 3.0f);
                AudioManager.Singleton.FadeOut("OceanMusic", 3.0f);
                AudioManager.Singleton.FadeIn("SnowMountainsMusic", 5.0f);
                AudioManager.Singleton.Stop("Waves");
                break;
            default:
                break;
        }
    }
    private void ChangeBackground()
    {
        switch (NextLocationState)
        {
            case LocationState.Desert:
                desertBackground.SetActive(true);
                oceanBackground.SetActive(false);
                snowMountainsBackground.SetActive(false);
                break;
            case LocationState.Ocean:
                desertBackground.SetActive(false);
                oceanBackground.SetActive(true);
                snowMountainsBackground.SetActive(false);
                break;
            case LocationState.SnowMountains:
                desertBackground.SetActive(false);
                oceanBackground.SetActive(false);
                snowMountainsBackground.SetActive(true);
                break;
            default:
                break;
        }
    }
}
