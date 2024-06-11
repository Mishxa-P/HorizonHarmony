using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

public class Saves : MonoBehaviour
{
    private static string gameDataPath;
    private static string gameSettingsPath;
    private static bool initLoad = false;

    private static Saves instance;

    private void Awake()
    {
        if (!initLoad)
        {
            gameDataPath = Application.persistentDataPath + "/data.json";
            gameSettingsPath = Application.persistentDataPath + "/settings.json";
            LoadGameData();
            LoadGameSettings();
            initLoad = true;
        }
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }
    private class SaveGameData
    {
        public int currentTotalCoins;
        public int totalCoinsCollected;
        public TimeSpan totalGameDuration;

        public uint maxDistance;
        public uint totalDistance;
        public uint totalSlideDistance;

        public uint totalDeathCount;
        public uint totalLocationChangedCount;

        public uint totalObstaclesDestroyed;
        public uint totalTricksCount;
    }

    private class SaveGameSettings
    {
        public bool inversion;
        public float globalVolume;
        public float musicVolume;
        public float soundsVolume;
        public bool firstLanguageLoad;

        public ModesMenu.GameMode gameMode;
        public ModesMenu.LocationMode locationMode;
        public bool randomInitialLocState;
        public MapGenerationManager.LocationState initialLocState;

        public LanguageMenu.GameLanguage gameLanguage;
    }

    public void Save()
    {
        SaveGameData gameData = new SaveGameData()
        {
            currentTotalCoins = GameData.currentTotalCoins.GetCoins(),
            totalCoinsCollected = GameData.totalCoinsCollected.GetCoins(),
            totalGameDuration = GameData.totalGameDuration,

            maxDistance = GameData.maxDistance,
            totalDistance = GameData.totalDistance,
            totalSlideDistance = GameData.totalSlideDistance,

            totalDeathCount = GameData.totalDeathCount,
            totalLocationChangedCount = GameData.totalLocationChangedCount,

            totalObstaclesDestroyed = GameData.totalObstaclesDestroyed,
            totalTricksCount = GameData.totalTricksCount
        };

        SaveGameSettings gameSettings = new SaveGameSettings()
        {
            firstLanguageLoad = GameSettings.firstLanguageLoad,
            inversion = GameSettings.inversion,
            globalVolume = GameSettings.globalVolume,
            musicVolume = GameSettings.musicVolume,
            soundsVolume = GameSettings.soundsVolume,
            gameMode = GameSettings.gameMode,
            locationMode = GameSettings.locationMode,
            randomInitialLocState = GameSettings.randomInitialLocState,
            initialLocState = GameSettings.initialLocState,
            gameLanguage = GameSettings.gameLanguage
        };

       File.WriteAllText(gameDataPath,
             CryptoEngine.Encrypt(
                    JsonConvert.SerializeObject(gameData, Formatting.None, new JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    }),
                    CryptoEngine.key)
        );

        File.WriteAllText(gameSettingsPath,
             JsonConvert.SerializeObject(gameSettings, Formatting.None, new JsonSerializerSettings()
             {
                 ReferenceLoopHandling = ReferenceLoopHandling.Ignore
             })
        );

        Debug.Log("Game data and settings are saved!");
    }

    private void LoadGameData()
    {
        SaveGameData gameData = null;
        if (File.Exists(gameDataPath))
        {
            try
            {
                gameData = JsonConvert.DeserializeObject<SaveGameData>(CryptoEngine.Decrypt(File.ReadAllText(gameDataPath), CryptoEngine.key));
            }
            catch (Exception e)
            {
                Debug.Log("Exception when decrypting a save file: " + e.ToString());
                SetDefaultGameDataValues();
                return;
            }
            if (gameData == null)
            {
                Debug.Log("Incorect saved data file!");
                SetDefaultGameDataValues();
                return;
            }

            GameData.localCoins = new Coin(0);
            GameData.currentTotalCoins = new Coin(gameData.currentTotalCoins);
            GameData.totalCoinsCollected = new Coin(gameData.totalCoinsCollected);
            GameData.totalGameDuration = gameData.totalGameDuration;

            GameData.maxDistance = gameData.maxDistance;
            GameData.totalDistance = gameData.totalDistance;
            GameData.totalSlideDistance = gameData.totalSlideDistance;

            GameData.totalDeathCount = gameData.totalDeathCount;
            GameData.totalLocationChangedCount = gameData.totalLocationChangedCount;

            GameData.totalObstaclesDestroyed = gameData.totalObstaclesDestroyed;
            GameData.totalTricksCount = gameData.totalTricksCount;
            Debug.Log("Game data is loaded!");
        }
        else
        {
            Debug.Log("Game data file is not found!");
            SetDefaultGameDataValues();
        }
        GameData.dayTimeState = (DayTime.State)UnityEngine.Random.Range(0, 2);
        GameData.dayTimeStartTime = 0.0f;
    }

    private void LoadGameSettings()
    {
        SaveGameSettings gameSettings = null;
        if (File.Exists(gameSettingsPath))
        {
            try
            {
                gameSettings = JsonConvert.DeserializeObject<SaveGameSettings>(File.ReadAllText(gameSettingsPath));
            }
            catch (JsonReaderException e)
            {
                Debug.Log("Exception when reading a saved settings file: " + e.ToString());
                SetDefaultGameSettingsValues();
                return;
            }
            if (gameSettings == null)
            {
                Debug.Log("Incorect saved settings file!");
                SetDefaultGameSettingsValues();
                return;
            }

            GameSettings.inversion = gameSettings.inversion;
            GameSettings.globalVolume = gameSettings.globalVolume;
            GameSettings.soundsVolume = gameSettings.soundsVolume;
            GameSettings.musicVolume = gameSettings.musicVolume;
            GameSettings.firstLanguageLoad = gameSettings.firstLanguageLoad;
            GameSettings.gameMode = gameSettings.gameMode;
            GameSettings.locationMode = gameSettings.locationMode;
            GameSettings.randomInitialLocState = gameSettings.randomInitialLocState;
            GameSettings.initialLocState = gameSettings.initialLocState;
            GameSettings.gameLanguage = gameSettings.gameLanguage;

            Debug.Log("Game settings are loaded!");
        }
        else
        {
            Debug.Log("Game settings file is not found!");
            SetDefaultGameSettingsValues();
        }
    }

    private static void SetDefaultGameDataValues()
    {
        GameData.localCoins = new Coin(0);
        GameData.currentTotalCoins = new Coin(0);
        GameData.totalCoinsCollected = new Coin(0);
        GameData.totalGameDuration = new TimeSpan(0, 0, 0, 0);
        GameData.maxDistance = 0;
        GameData.totalDistance = 0;
        GameData.totalDeathCount = 0;
        GameData.totalSlideDistance = 0;
        GameData.totalLocationChangedCount = 0;
        GameData.totalObstaclesDestroyed = 0;
        GameData.totalTricksCount = 0;
        Debug.Log("Using default game data values!");
    }

    private static void SetDefaultGameSettingsValues()
    {
        GameSettings.firstLanguageLoad = true;
        GameSettings.inversion = false;
        GameSettings.globalVolume = 0.5f;
        GameSettings.musicVolume = 0.5f;
        GameSettings.soundsVolume = 0.5f;
        GameSettings.gameMode = ModesMenu.GameMode.Default;
        GameSettings.locationMode = ModesMenu.LocationMode.Default;
        GameSettings.randomInitialLocState = true;
        Debug.Log("Using default game settings values!");
    }
    private class CryptoEngine
    {
        public static string key = "pit--adve-nturer"; //128 or 192 bit

        public static string Encrypt(string input, string key)
        {
            byte[] inputArray = UTF8Encoding.UTF8.GetBytes(input);
            TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
            tripleDES.Key = UTF8Encoding.UTF8.GetBytes(key);
            tripleDES.Mode = CipherMode.ECB;
            tripleDES.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tripleDES.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            tripleDES.Clear();
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        public static string Decrypt(string input, string key)
        {
            try
            {
                byte[] inputArray = Convert.FromBase64String(input);
                TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
                tripleDES.Key = UTF8Encoding.UTF8.GetBytes(key);
                tripleDES.Mode = CipherMode.ECB;
                tripleDES.Padding = PaddingMode.PKCS7;
                ICryptoTransform cTransform = tripleDES.CreateDecryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
                tripleDES.Clear();
                return UTF8Encoding.UTF8.GetString(resultArray);
            }
            catch (Exception e)
            {
                Debug.Log("Exception when reading a saved data file: " + e.ToString());
                return null;
            }
        }
    }
    private void OnApplicationQuit()
    {
        Save();
    }
 }
