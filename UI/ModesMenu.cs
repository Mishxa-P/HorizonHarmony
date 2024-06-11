using System;
using TMPro;
using UnityEngine;

public class ModesMenu : MonoBehaviour
{
    [SerializeField] private GameMode gameMode;
    [SerializeField] private LocationMode locationMode;
    [SerializeField] private MapGenerationManager.LocationState location;
    [SerializeField] private TMP_Text gameModeTMP;
    [SerializeField] private TMP_Text locationModeTMP;
    [SerializeField] private TMP_Text locationTMP;
    [SerializeField] private GameObject locationUI;
   
    MainMenu mainMenu;

    private bool initializingLocalizationInstance = false;

    public enum GameMode
    {
        Default = 0,
        Relax = 1
    }
    public enum LocationMode
    {
        Default = 0,
        OneLocation = 1
    }

    private int currentGameModeIndex = 0;
    private int currentLocationModeIndex = 0;
    private int currentLocationIndex = 0;
    private void Start()
    {
        mainMenu = GetComponentInParent<MainMenu>();
    }
    private void Update()
    {
        if (initializingLocalizationInstance)
        {
            UpdateModesText();
        }
    }
    public void UpdateModes()
    {
        currentGameModeIndex = (int)GameSettings.gameMode;
        currentLocationModeIndex = (int)GameSettings.locationMode;
        currentLocationIndex = (int)GameSettings.initialLocState;
    }
    public void UpdateModesText()
    {
        gameModeTMP.text = GetStringFromTable("gameMode_" + GameSettings.gameMode.ToString());
        locationModeTMP.text = GetStringFromTable("locationMode_" + GameSettings.locationMode.ToString());
        locationTMP.text = GetStringFromTable("location_" + GameSettings.initialLocState.ToString());
        if (GameSettings.locationMode == LocationMode.OneLocation)
        {
            locationUI.SetActive(true);
        }
    }
    public void ChangeGameMode(bool rightDirection = true)
    {
        if (rightDirection)
        {
            if (currentGameModeIndex < Enum.GetNames(typeof(GameMode)).Length - 1)
            {
                currentGameModeIndex++;
            }
            else
            {
                currentGameModeIndex = 0;
            }
        }
        else
        {
            if (currentGameModeIndex > 0)
            {
                currentGameModeIndex--;
            }
            else
            {
                currentGameModeIndex = Enum.GetNames(typeof(GameMode)).Length - 1;
            }
        }
        gameMode = (GameMode)currentGameModeIndex;
        GameSettings.gameMode = gameMode;
        gameModeTMP.text = GetStringFromTable("gameMode_" + gameMode.ToString());
    }

    public void ChangeLocationMode(bool rightDirection = true)
    {
        if (rightDirection)
        {
            if (currentLocationModeIndex < Enum.GetNames(typeof(LocationMode)).Length - 1)
            {
                currentLocationModeIndex++;
            }
            else
            {
                currentLocationModeIndex = 0;
            }
        }
        else
        {
            if (currentLocationModeIndex > 0)
            {
                currentLocationModeIndex--;
            }
            else
            {
                currentLocationModeIndex = Enum.GetNames(typeof(LocationMode)).Length - 1;
            }
        }
        locationMode = (LocationMode)currentLocationModeIndex;
        GameSettings.locationMode = locationMode;
        locationModeTMP.text = GetStringFromTable("locationMode_" + locationMode.ToString());
        if (locationMode == LocationMode.OneLocation)
        {
            locationUI.SetActive(true);
        }
        else
        {
            locationUI.SetActive(false);
        }
    }

    public void ChangeLocation(bool rightDirection = true)
    {
        if (rightDirection)
        {
            if (currentLocationIndex < Enum.GetNames(typeof(MapGenerationManager.LocationState)).Length - 1)
            {
                currentLocationIndex++;
            }
            else
            {
                currentLocationIndex = 0;
            }
        }
        else
        {
            if (currentLocationIndex > 0)
            {
                currentLocationIndex--;
            }
            else
            {
                currentLocationIndex = Enum.GetNames(typeof(MapGenerationManager.LocationState)).Length - 1;
            }
        }
        location = (MapGenerationManager.LocationState)currentLocationIndex;
        GameSettings.initialLocState = location;
        locationTMP.text = GetStringFromTable("location_" + location.ToString());
        mainMenu.ChangeBackground(location);
    }
    private string GetStringFromTable(string key)
    {
        if (LanguageManager.Singleton == null)
        {
            initializingLocalizationInstance = true;
            return "----";
        }
        initializingLocalizationInstance = false;
        return LanguageManager.Singleton.GetStringDatabase().GetLocalizedString(key);
    }
}
