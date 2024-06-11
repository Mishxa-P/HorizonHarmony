using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject resultScreen;
    [SerializeField] private GameObject blur;
    [SerializeField] private GameObject HUD;

    private void OnEnable()
    {
        PlayerDeath.onPlayerDied += ShowResultScreen;
    }
    private void OnDisable()
    {
        PlayerDeath.onPlayerDied -= ShowResultScreen;
    }
    public void PauseGame()
    {
        Debug.Log("Pause");
        Time.timeScale = 0.0f;
        Timer.Pause();
        PlayerInputManager.Singleton.DisableInput();
    }
    public void UnpauseGame()
    {
        Debug.Log("Unpause");
        Time.timeScale = 1.0f;
        Timer.Unpause();
        PlayerInputManager.Singleton.EnableInput();
    }
    public void OnButtonClick()
    {
        AudioManager.Singleton.Play("Click");
    }
    public void ReloadGame()
    {
        if (GameSettings.locationMode != ModesMenu.LocationMode.OneLocation)
        {
            GameSettings.randomInitialLocState = true;
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void LoadMainMenuScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);

        AudioManager.Singleton.StopAll();
    }
    private void ShowResultScreen()
    {
        Timer.End();
        blur.SetActive(true);
        HUD.SetActive(false);
        resultScreen.SetActive(true);
        resultScreen.GetComponent<Result>().UpdateResult();
    }
}
