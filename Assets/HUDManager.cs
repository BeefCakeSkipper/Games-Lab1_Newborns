
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class HUDManager : MonoBehaviour
{
    [Header("HUD References")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;
    public GameObject gameOverScreen;
    public TextMeshProUGUI resultText;

    public Vector2 gameOverScorePosition = new Vector2(0, -80);

    private Canvas canvas;

    // Original score position
    private Vector2 scoreAnchorMin;
    private Vector2 scoreAnchorMax;
    private Vector2 scorePosition;
    private TextAlignmentOptions scoreAlignment;
    private int scoreSiblingIndex;

    private void Awake()
    {


        canvas = GetComponent<Canvas>();

        if (canvas != null &&
            canvas.renderMode == RenderMode.ScreenSpaceCamera &&
            canvas.worldCamera == null)
        {
            canvas.worldCamera = Camera.main;
        }

        SceneManager.activeSceneChanged += UseSceneCamera;
    }

    private void Start()
    {
        // Remember the original score position.
        RectTransform scoreRect = scoreText.rectTransform;

        scoreAnchorMin = scoreRect.anchorMin;
        scoreAnchorMax = scoreRect.anchorMax;
        scorePosition = scoreRect.anchoredPosition;
        scoreAlignment = scoreText.alignment;
        scoreSiblingIndex = scoreRect.GetSiblingIndex();

        gameOverScreen.SetActive(false);
    }

    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= UseSceneCamera;
    }
    // Update the Canvas camera when changing scenes.
    private void UseSceneCamera(Scene current, Scene next)
    {
        if (canvas != null &&
            canvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            canvas.worldCamera = Camera.main;
        }
    }

    // Called by OnUpdateScore.asset
    public void SetScore(int score)
    {
        Debug.Log($"[HUDManager] SetScore received: {score}");

        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }

    // Called by OnUpdateHighScore.asset
    public void SetHighScore(int highScore)
    {
        Debug.Log($"HUDManager: Received HighScore={highScore}");
        if (highScoreText != null)
        {
            highScoreText.text = "High Score: " + highScore;
        }
    }

    // Called by OnGameOver.asset
    public void ShowGameOver()
    {
        ShowEndScreen("GAME OVER");
    }

    public void ShowEndScreen(string message)
    {
        resultText.text = message;
        gameOverScreen.SetActive(true);

        // Move score below the game-over text.
        RectTransform scoreRect = scoreText.rectTransform;

        scoreRect.anchorMin = new Vector2(0.5f, 0.5f);
        scoreRect.anchorMax = new Vector2(0.5f, 0.5f);
        scoreRect.anchoredPosition = gameOverScorePosition;

        scoreText.alignment = TextAlignmentOptions.Center;
        scoreRect.SetAsLastSibling();
    }

    // Called by OnGameRestart.asset
    public void GameRestart()
    {
        gameOverScreen.SetActive(false);

        // Restore the original score position.
        RectTransform scoreRect = scoreText.rectTransform;

        scoreRect.anchorMin = scoreAnchorMin;
        scoreRect.anchorMax = scoreAnchorMax;
        scoreRect.anchoredPosition = scorePosition;

        scoreText.alignment = scoreAlignment;
        scoreRect.SetSiblingIndex(scoreSiblingIndex);
    }
    public void ShowLevelComplete()
    {
        ShowEndScreen("You Win!");
    }
}
