
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject pauseMenu;

    [Header("ScriptableObject Events")]
    [SerializeField] private SimpleGameEvent onGamePause;
    [SerializeField] private SimpleGameEvent onGameResume;
    [SerializeField] private SimpleGameEvent onReturnToMainMenu;
    [SerializeField] private PlayerMovement player;
    public bool isLevelComplete = false;

    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (player != null && player.isDead)
                return;

            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        if (player != null && (player.isDead || player.isLevelComplete))
            return;

        if (isPaused)
            return;

        pauseMenu.SetActive(true);

        Time.timeScale = 0f;
        AudioListener.pause = true;

        isPaused = true;

        if (onGamePause != null)
            onGamePause.Raise();
    }

    public void ResumeGame()
    {
        if (!isPaused)
            return;

        pauseMenu.SetActive(false);

        Time.timeScale = 1f;
        AudioListener.pause = false;

        isPaused = false;

        if (onGameResume != null)
            onGameResume.Raise();
    }

    public void ReturnToMainMenu()
    {
        Debug.Log("ReturnToMainMenu called");

        Time.timeScale = 1f;
        AudioListener.pause = false;

        Debug.Log("Attempting to load MainMenu");

        SceneManager.LoadScene("MainMenu");
    }
}
