using System;
using UnityEngine;

public class ForegroundAnimationEvent : MonoBehaviour
{
    [SerializeField] private bool desertEvent;
    [SerializeField] private bool mainMenu = false;
    private Parallax parallax;

    public static Action OnForegroundEventPlayed;

    private void Start()
    {
        parallax = GetComponent<Parallax>();
    }
    private void Update()
    {
        if (desertEvent)
        {
            if (parallax.offset.x <= -58.0f)
            {
                ChangeLocationThroughAnimationEvent();
                desertEvent = false;
            }
        }
    }
    public void PlayDesertForegroundBigEventThroughAnimationEvent()
    {
        ForegroundEventManager.Singleton.PlayDesertForegroundBigEvent();
    }
    public void PlayOceanForegroundBigEventThroughAnimationEvent()
    {
        ForegroundEventManager.Singleton.PlayOceanForegroundBigEvent();
    }
    public void PlaySnowMountainsForegroundBigEventThroughAnimationEvent()
    {
        ForegroundEventManager.Singleton.PlaySnowMountainsForegroundBigEvent();
    }
    public void ChangeLocationThroughAnimationEvent()
    {
        if (mainMenu)
        {
            OnForegroundEventPlayed?.Invoke();
        }
        else
        {
            MapGenerationManager.Singleton.ChangeLocation();
        }
    }
}
