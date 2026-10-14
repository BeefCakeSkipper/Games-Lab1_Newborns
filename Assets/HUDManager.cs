using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

// owns the HUD (score, high score, game over screen) and updates it from GameManager events,
// it survives scene changes like mario and the GameManager so the score carries over to the next level
public class HUDManager : Singleton<HUDManager>
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;
    public GameObject gameOverScreen;
    public TextMeshProUGUI resultText;
    public Vector2 gameOverScorePosition = new Vector2(0, -80); // below the game over text

    private Canvas canvas;

    // original score position
    private Vector2 scoreAnchorMin;
    private Vector2 scoreAnchorMax;
    private Vector2 scorePosition;
    private TextAlignmentOptions scoreAlignment;
    private int scoreSiblingIndex;

    public override void Awake()
    {
        base.Awake();
        canvas = GetComponent<Canvas>();
        // a prefab can't keep a link to the scene camera, so grab it here too
        if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceCamera && canvas.worldCamera == null)
            canvas.worldCamera = Camera.main;
        SceneManager.activeSceneChanged += UseSceneCamera;
    }

    void Start()
    {
        // remember where the score sits in the HUD
        RectTransform scoreRect = scoreText.rectTransform;
        scoreAnchorMin = scoreRect.anchorMin;
        scoreAnchorMax = scoreRect.anchorMax;
        scorePosition = scoreRect.anchoredPosition;
        scoreAlignment = scoreText.alignment;
        scoreSiblingIndex = scoreRect.GetSiblingIndex();

        gameOverScreen.SetActive(false);
        UpdateHighScoreText();

        GameManager.instance.scoreChange.AddListener(SetScore);
        GameManager.instance.gameRestart.AddListener(GameRestart);
    }

    protected override void OnDestroy()
    {
        SceneManager.activeSceneChanged -= UseSceneCamera;
        if (GameManager.instance != null)
        {
            GameManager.instance.scoreChange.RemoveListener(SetScore);
            GameManager.instance.gameRestart.RemoveListener(GameRestart);
        }
        base.OnDestroy();
    }

    // the canvas renders through the scene camera, so switch to the new scene's camera after a scene change
    void UseSceneCamera(Scene current, Scene next)
    {
        if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceCamera)
            canvas.worldCamera = Camera.main;
    }

    public void SetScore(int score)
    {
        scoreText.text = "Score: " + score.ToString();
        UpdateHighScoreText();
    }

    void UpdateHighScoreText()
    {
        if (highScoreText != null)
            highScoreText.text = "High Score: " + GameManager.instance.highScore.ToString();
    }

    public void ShowEndScreen(string message)
    {
        resultText.text = message;
        gameOverScreen.SetActive(true);

        // move the score to the center and below the game over text
        RectTransform scoreRect = scoreText.rectTransform;
        scoreRect.anchorMin = new Vector2(0.5f, 0.5f);
        scoreRect.anchorMax = new Vector2(0.5f, 0.5f);
        scoreRect.anchoredPosition = gameOverScorePosition;
        scoreText.alignment = TextAlignmentOptions.Center;
        scoreRect.SetAsLastSibling(); // put score over the gameover overlay
    }

    void GameRestart()
    {
        gameOverScreen.SetActive(false);

        // put the score back to the corner
        RectTransform scoreRect = scoreText.rectTransform;
        scoreRect.anchorMin = scoreAnchorMin;
        scoreRect.anchorMax = scoreAnchorMax;
        scoreRect.anchoredPosition = scorePosition;
        scoreText.alignment = scoreAlignment;
        scoreRect.SetSiblingIndex(scoreSiblingIndex);
    }
}
