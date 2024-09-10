using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private float backgroundChangeTime = 30.0f;
    [Header("Background")]
    [SerializeField] private GameObject desertBackground;
    [SerializeField] private GameObject oceanBackground;
    [SerializeField] private GameObject snowMountainsBackground;
    [SerializeField] private CanvasGroup mainCanvasGroup;
    private bool canChangeBackground = true;
    private bool backgroundWasChaged = false;

    private bool firstChange = true;

    MapGenerationManager.LocationState nextLocationState;
    MapGenerationManager.LocationState currentLocationState;

    private void OnEnable()
    {
        ForegroundAnimationEvent.OnForegroundEventPlayed += ChangeBackgroundaAfterForegroundEvent;
    }
    private void OnDisable()
    {
        ForegroundAnimationEvent.OnForegroundEventPlayed -= ChangeBackgroundaAfterForegroundEvent;
    }
    private void Start()
    {
        Application.targetFrameRate = 60;
        AudioManager.Singleton.StopAll();
        Time.timeScale = 1.0f;
        StartCoroutine(PlayMenuMusic());
    }
    private IEnumerator PlayMenuMusic()
    {
        yield return new WaitForSeconds(1.0f);
        AudioManager.Singleton.FadeIn("MenuMusic", 3.0f);
    }
    private void Update()
    {
        if (canChangeBackground && GameSettings.locationMode != ModesMenu.LocationMode.OneLocation)
        {
            StartCoroutine(ActivateForegroundEvent());
        }
        else if (!backgroundWasChaged)
        {
            backgroundWasChaged = true;
            ChangeBackground(GameSettings.initialLocState);
        }
    }
    public void StartGame()
    {
        mainCanvasGroup.blocksRaycasts = false;
        StartCoroutine(LoadGameSceneAsync());
    }
    private IEnumerator LoadGameSceneAsync()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
    public void OnButtonClick()
    {
        AudioManager.Singleton.Play("Click");
    }
    public void ChangeBackground(MapGenerationManager.LocationState locationState)
    {
        currentLocationState = locationState;
        switch (currentLocationState)
        {
            case MapGenerationManager.LocationState.Desert:
                desertBackground.SetActive(true);
                oceanBackground.SetActive(false);
                snowMountainsBackground.SetActive(false);
                break;
            case MapGenerationManager.LocationState.Ocean:
                desertBackground.SetActive(false);
                oceanBackground.SetActive(true);
                snowMountainsBackground.SetActive(false);
                break;
            case MapGenerationManager.LocationState.SnowMountains:
                desertBackground.SetActive(false);
                oceanBackground.SetActive(false);
                snowMountainsBackground.SetActive(true);
                break;
            default:
                break;
        }
        GameSettings.initialLocState = currentLocationState;
        GameSettings.randomInitialLocState = false;
    }
    public void QuitGame()
    {
        Debug.Log("Quit!");
        Application.Quit();
    }
    private IEnumerator ActivateForegroundEvent()
    {
        canChangeBackground = false;

        nextLocationState = currentLocationState;

        int newLocationState = Random.Range(0, 3);

        while ((int)nextLocationState == newLocationState)
        {
            newLocationState = Random.Range(0, 3);
        }
        nextLocationState = (MapGenerationManager.LocationState)newLocationState;

        if (firstChange)
        {
            ChangeBackground(nextLocationState);
            firstChange = false;
        }
        else
        {
            switch (nextLocationState)
            {
                case MapGenerationManager.LocationState.Desert:
                    ForegroundEventManager.Singleton.PlayDesertForegroundBigEvent();
                    break;
                case MapGenerationManager.LocationState.Ocean:
                    ForegroundEventManager.Singleton.PlayOceanForegroundBigEvent();
                    break;
                case MapGenerationManager.LocationState.SnowMountains:
                    ForegroundEventManager.Singleton.PlaySnowMountainsForegroundBigEvent();
                    break;
                default:
                    break;
            }
        }

        yield return new WaitForSeconds(backgroundChangeTime);
        canChangeBackground = true;
    }
    private void ChangeBackgroundaAfterForegroundEvent()
    {
        ChangeBackground(nextLocationState);
    }
    private void OnDestroy()
    {
        if (AudioManager.Singleton != null)
        {
            AudioManager.Singleton.Stop("MenuMusic");
        }
    }
}
