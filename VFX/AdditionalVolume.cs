
using UnityEngine;
using UnityEngine.Rendering;

public class AdditionalVolume: MonoBehaviour
{
    private void OnEnable()
    {
        SettingsMenu.onVFXSettingChanged.AddListener(UpdateVolume);
    }
    private void OnDisable()
    {
        SettingsMenu.onVFXSettingChanged.RemoveListener(UpdateVolume);
    }

    private void Start()
    {
        UpdateVolume();
    }
    private void UpdateVolume()
    {
        if (GameSettings.visualEffects)
        {
            GetComponent<Volume>().enabled = true;
        }
        else
        {
            GetComponent<Volume>().enabled = false;
        }
    }
}
