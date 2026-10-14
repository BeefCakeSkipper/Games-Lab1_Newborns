using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenu;

    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (PlayerMovement.instance != null &&
                PlayerMovement.instance.isDead)
                return;

            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        if (PlayerMovement.instance != null &&
            PlayerMovement.instance.isDead)
            return;

        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        AudioListener.pause = true;

        isPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        AudioListener.pause = false;

        isPaused = false;
    }

    public void ReturnToMainMenu()
    {
        // Resume game and audio
        Time.timeScale = 1f;
        AudioListener.pause = false;
        isPaused = false;

        // Destroy persistent Mario
        if (PlayerMovement.instance != null)
        {
            Destroy(PlayerMovement.instance.gameObject);
        }

        // Return to Main Menu
        SceneManager.LoadScene("MainMenu");
    }
}