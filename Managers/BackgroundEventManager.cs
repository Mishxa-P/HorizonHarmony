using System.Collections.Generic;
using UnityEngine;
using static MapGenerationManager;

public class BackgroundEventManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> desertEvents;
    [SerializeField] private List<GameObject> oceanEvents;
    [SerializeField] private List<GameObject> snowMountainsEvents;

    public static BackgroundEventManager Singleton { get; private set; }

    private int previousEventIndex = -1;
    private void Awake()
    {
        if (Singleton != null)
        {
            Debug.LogError("Background event manager is already exist!");
        }
        else
        {
            Singleton = this;
        }
    }
    public float GetGoalMetersForNewEvent(float nextLocationGoalMeters)
    {
        return Random.Range(10.0f, nextLocationGoalMeters * 0.65f);
    }
    public void ActivateRandomEvent(LocationState locationState)
    {
        Debug.Log($"Activating {locationState} event");
        switch (locationState)
        {
            case LocationState.Desert:
                if (desertEvents.Count > 1)
                {
                    int index = Random.Range(0, desertEvents.Count);
                    while (previousEventIndex == index)
                    {
                        index = Random.Range(0, desertEvents.Count);
                    }
                    desertEvents[index].SetActive(true);
                    desertEvents[index].GetComponent<Parallax>().ActivateBackgroundEvent();
                    previousEventIndex = index;
                }
                break;
            case LocationState.Ocean:
                if (oceanEvents.Count > 1)
                {
                    int index = Random.Range(0, oceanEvents.Count);
                    while (previousEventIndex == index)
                    {
                        index = Random.Range(0, oceanEvents.Count);
                    }
                    oceanEvents[index].SetActive(true);
                    oceanEvents[index].GetComponent<Parallax>().ActivateBackgroundEvent();
                    previousEventIndex = index;
                }
                break;
            case LocationState.SnowMountains:
                if (snowMountainsEvents.Count > 1)
                {
                    int index = Random.Range(0, snowMountainsEvents.Count);
                    while (previousEventIndex == index)
                    {
                        index = Random.Range(0, snowMountainsEvents.Count);
                    }
                    snowMountainsEvents[index].SetActive(true);
                    snowMountainsEvents[index].GetComponent<Parallax>().ActivateBackgroundEvent();
                    previousEventIndex = index;
                }
                break;
            default:
                break;
        }
       
    }
    public void DeactivateAllEvents()
    {
        Debug.Log($"Deactivating all location`s events");
        foreach (GameObject go in desertEvents)
        {
            go.SetActive(false);
        }
        foreach (GameObject go in oceanEvents)
        {
            go.SetActive(false);
        }
        foreach (GameObject go in snowMountainsEvents)
        {
            go.SetActive(false);
        }
    }
}
