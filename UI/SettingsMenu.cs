using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private Toggle inversion;
    [SerializeField] private Toggle visualEffects;
    [SerializeField] private RectTransform developersFrame;
    [SerializeField] private Slider globalVolume;
    [SerializeField] private Slider musicVolume;
    [SerializeField] private Slider soundsVolume;

    //parameters for localization
    private const float RUSSIAN_DEV_FRAME_WIDTH = 325.0f;
    private const float RUSSIAN_DEV_FRAME_HEIGHT = 70.0f;
    private const float RUSSIAN_DEV_FRAME_POS_X = -75.0f;
    private const float ENGLISH_DEV_FRAME_WIDTH = 260.0f;
    private const float ENGLISH_DEV_FRAME_HEIGHT = 65.0f;
    private const float ENGLISH_DEV_FRAME_POS_X = -110.0f;

    public static UnityEvent onVFXSettingChanged = new UnityEvent();

    private bool firstUpdate = true;
    private void OnEnable()
    {
        inversion.onValueChanged.AddListener(SetInversion);
        visualEffects.onValueChanged.AddListener(SetVFX);
        globalVolume.onValueChanged.AddListener(SetGlobalVolume);
        musicVolume.onValueChanged.AddListener(SetMusicVolume);
        soundsVolume.onValueChanged.AddListener(SetSoundsVolume);
    }
    private void OnDisable()
    {
        inversion.onValueChanged.RemoveListener(SetInversion);
        visualEffects.onValueChanged.RemoveListener(SetVFX);
        globalVolume.onValueChanged.RemoveListener(SetGlobalVolume);
        musicVolume.onValueChanged.RemoveListener(SetMusicVolume);
        soundsVolume.onValueChanged.RemoveListener(SetSoundsVolume);
    }
    public void UpdateSettings()
    {
        inversion.isOn = GameSettings.inversion;
        visualEffects.isOn = GameSettings.visualEffects;
        globalVolume.value = GameSettings.globalVolume;
        musicVolume.value = GameSettings.musicVolume;
        soundsVolume.value = GameSettings.soundsVolume;
        firstUpdate = false;
    }
    private void SetInversion(bool isOn)
    {
        if (!firstUpdate)
        {
            AudioManager.Singleton.Play("Click");
        }
        GameSettings.inversion = isOn;
    }
    private void SetVFX(bool isOn)
    {
        if (!firstUpdate)
        {
            AudioManager.Singleton.Play("Click");
        }
        GameSettings.visualEffects = isOn;
        onVFXSettingChanged?.Invoke();
    }
    private void SetGlobalVolume(float value)
    {
        GameSettings.globalVolume = value;
        AudioManager.Singleton.UpdateVolumeSettings();
    }
    private void SetMusicVolume(float value)
    {
        GameSettings.musicVolume = value;
        AudioManager.Singleton.UpdateVolumeSettings();
    }
    private void SetSoundsVolume(float value)
    {
        GameSettings.soundsVolume = value;
        AudioManager.Singleton.UpdateVolumeSettings();
    }
    public void UpdateDevelopersFrame()
    {
        switch (GameSettings.gameLanguage)
        {
            case LanguageMenu.GameLanguage.English:
                developersFrame.anchoredPosition = new Vector3(ENGLISH_DEV_FRAME_POS_X, 0.0f, 0.0f);
                developersFrame.sizeDelta = new Vector2(ENGLISH_DEV_FRAME_WIDTH, ENGLISH_DEV_FRAME_HEIGHT);
                break;
            case LanguageMenu.GameLanguage.Russian:
                developersFrame.anchoredPosition = new Vector3(RUSSIAN_DEV_FRAME_POS_X, 0.0f, 0.0f);
                developersFrame.sizeDelta = new Vector2(RUSSIAN_DEV_FRAME_WIDTH, RUSSIAN_DEV_FRAME_HEIGHT);
                break;
            default:
                break;
        }
    }
}
