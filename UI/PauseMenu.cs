using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private CanvasGroup pauseScreen;
    [SerializeField] private CanvasGroup resultScreen;
    [SerializeField] private GameObject blur;
    [SerializeField] private CanvasGroup staticHUD;
    [SerializeField] private CanvasGroup activeHUD;

    private void OnEnable()
    {
        PlayerDeathZone.onPlayerDied += ShowResultScreen;
    }
    private void OnDisable()
    {
        PlayerDeathZone.onPlayerDied -= ShowResultScreen;
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
        StartCoroutine(LoadSceneAsync(SceneManager.GetActiveScene().buildIndex));
    }
    public void LoadMainMenuScene()
    {
        StartCoroutine(LoadSceneAsync(SceneManager.GetActiveScene().buildIndex - 1));
    }
    private IEnumerator LoadSceneAsync(int buildIndex)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(buildIndex);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
    private void ShowResultScreen()
    {
        Timer.End();
        blur.SetActive(true);
        pauseScreen.alpha = 0.0f;
        pauseScreen.blocksRaycasts = false;
        staticHUD.alpha = 0.0f;
        staticHUD.blocksRaycasts = false;
        activeHUD.alpha = 0.0f;
        resultScreen.alpha = 1.0f;
        resultScreen.blocksRaycasts = true;
        resultScreen.GetComponent<Result>().UpdateResult();
    }
}
