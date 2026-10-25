using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartButtonController : MonoBehaviour, IInteractiveButton
{
    [SerializeField] private SimpleGameEvent onGameRestart;
    [SerializeField] private PlayerMovement player;
    [SerializeField] private string mainMenuScene = "MainMenu";
    [SerializeField] private IntVariable gameScore;

    private void Start()
    {
        if (player == null)
        {
            player = FindFirstObjectByType<PlayerMovement>();
        }

        if (player == null)
        {
            Debug.LogError("[RestartButtonController] PlayerMovement not found!");
        }
    }

    public void ButtonClick()
    {
        Debug.Log("Restart button clicked");

        bool hasWon = player != null && player.isLevelComplete;

        // Resume gameplay and audio.
        Time.timeScale = 1f;
        AudioListener.pause = false;

        if (hasWon)
        {
            SceneManager.LoadScene(mainMenuScene);
            return;
        }

        // Reset the score before restarting.
        if (gameScore == null)
        {
            Debug.LogError("[RestartButtonController] Game Score is not assigned!");
            return;
        }

        gameScore.ResetValue();

        Debug.Log($"[RestartButtonController] Score reset to {gameScore.Value}");

        if (onGameRestart != null)
        {
            onGameRestart.Raise();
        }
        else
        {
            Debug.LogError("OnGameRestart is not assigned!");
            return;
        }

        string currentLevel = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentLevel);
    }
}