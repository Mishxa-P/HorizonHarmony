using TMPro;
using UnityEngine;

public class LanguageMenu : MonoBehaviour
{
    [SerializeField] private TMP_Text russianTMP;
    [SerializeField] private TMP_Text englishTMP;

    public enum GameLanguage
    {
        English = 0,
        Russian = 1
    }
    private bool gameLanguageSwitchIsActive = false;

    public void UpdateLanguagesMark()
    {
        MarkSelectedLanguage(GameSettings.gameLanguage);
    }
    public void ChangeGameLanguage(int localeID)
    {
        if (gameLanguageSwitchIsActive)
        {
            return;
        }
        else
        {
            SetGameLanguage(localeID);
        }
    }

    private void MarkSelectedLanguage(GameLanguage language)
    {
        switch (language)
        {
            case GameLanguage.Russian:
                russianTMP.color = Color.yellow;
                englishTMP.color = Color.white;
                break;
            case GameLanguage.English:
                russianTMP.color = Color.white;
                englishTMP.color = Color.yellow;
                break;
        }
        GameSettings.gameLanguage = language;
    }

    private void SetGameLanguage(int localeID)
    {
        LanguageManager.Singleton.SetSelectedLocale(LanguageManager.Singleton.GetAvailableLocales().Locales[localeID]);
        MarkSelectedLanguage((GameLanguage)localeID);
        gameLanguageSwitchIsActive = false;
        Debug.Log("Current loaded locale = " + LanguageManager.Singleton.GetSelectedLocale());
    }
}
