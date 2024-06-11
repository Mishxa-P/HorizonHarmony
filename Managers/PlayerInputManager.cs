using System.Collections;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager Singleton { get; private set; }

    public PlayerInputActions InputActions { get; private set; }

    private int clickExitCount = 0;
    private void Awake()
    {
        if (Singleton == null)
        {
            InputActions = new PlayerInputActions();
            Singleton = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            clickExitCount++;
            StartCoroutine(ClickExitTime());

            if (clickExitCount > 1)
            {
                Debug.Log("Quit game!");
                Application.Quit();
            }
        }
    }       
    public void EnableInput()
    {
        InputActions.Enable();
    }
    public void DisableInput()
    {
        InputActions.Disable();
    }
    private IEnumerator ClickExitTime()
    {
        yield return new WaitForSeconds(0.5f);
        clickExitCount = 0;
    }
}
