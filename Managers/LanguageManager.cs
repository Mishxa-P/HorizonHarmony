using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LanguageManager : MonoBehaviour
{
    public static LocalizationSettings Singleton { get; private set; }

    private void Start()
    {
        if (Singleton != null)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        StartCoroutine(InitializeLocalizationSettings());
    }

    private IEnumerator InitializeLocalizationSettings()
    {
        yield return LocalizationSettings.InitializationOperation;

        Singleton = LocalizationSettings.InitializationOperation.Result;
        if (GameSettings.firstLanguageLoad)
        {
            GameSettings.gameLanguage = (LanguageMenu.GameLanguage)Singleton.GetSelectedLocale().SortOrder;
            GameSettings.firstLanguageLoad = false;
        }
        else
        {
            Singleton.SetSelectedLocale(Singleton.GetAvailableLocales().Locales[(int)GameSettings.gameLanguage]);
        }
        Debug.Log("Current loaded locale = " + Singleton.GetSelectedLocale());
    }
}
