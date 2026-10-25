
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameConstants gameConstants;

    [Header("ScriptableObject Events")]
    [SerializeField] private SimpleGameEvent onGameStart;
    [SerializeField] private SimpleGameEvent onGameRestart;
    [SerializeField] private IntGameEvent onUpdateScore;
    [SerializeField] private IntGameEvent onUpdateHighScore;
    [SerializeField] private SimpleGameEvent onGameOver;
    [SerializeField] private IntVariable gameScore;

    private const string HighScoreKey = "HighScore";
    public int highScore { get; private set; }

    private void Awake()
    {
        highScore = PlayerPrefs.GetInt(HighScoreKey, 0);

        if (gameConstants == null)
        {
            gameConstants = ScriptableObject.CreateInstance<GameConstants>();
        }

        if (gameScore == null)
        {
            Debug.LogError("[GameManager] Game Score IntVariable is not assigned!");
        }
    }

    private void Start()
    {
        Time.timeScale = 1.0f;

        onGameStart.Raise();

        // Display the existing score without resetting it.
        SetScore(gameScore.Value);
    }

    public void GameRestart()
    {
        Time.timeScale = 1.0f;

        SetScore(gameConstants.startingScore);

        onGameRestart.Raise();
    }

    public void IncreaseScore(int increment)
    {
        SetScore(gameScore.Value + increment);
    }

    public void ResetHighScore()
    {
        highScore = 0;

        PlayerPrefs.DeleteKey(HighScoreKey);
        PlayerPrefs.Save();

        onUpdateHighScore.Raise(highScore);
    }

    public void SetScore(int newScore)
    {
        gameScore.Value = newScore;

        if (gameScore.Value > highScore)
        {
            highScore = gameScore.Value;

            PlayerPrefs.SetInt(HighScoreKey, highScore);
            PlayerPrefs.Save();
        }

        onUpdateScore.Raise(gameScore.Value);
        onUpdateHighScore.Raise(highScore);
    }

    public void GameOver()
    {
        Time.timeScale = 0.0f;
        onGameOver.Raise();
    }

    // Call this when starting a completely new game.
    public void StartNewGame()
    {
        SetScore(gameConstants.startingScore);
    }
}
