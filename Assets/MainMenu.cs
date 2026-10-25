using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private IntVariable gameScore;
    public TextMeshProUGUI highScoreText;
    private const string HighScoreKey = "HighScore"; // same key PlayerMovement saves the high score under

    void Start()
    {
        gameScore.ResetValue();
        UpdateHighScoreText();
    }

    public void StartGame()
    {
        Debug.Log("Start button clicked!");
        SceneManager.LoadScene("LoadingScene");

    }

    // called by the reset highscore button
    public void ResetHighScore()
    {
        // the GameManager survives coming back to the menu and remembers the high score, so reset it there too
        if (gameManager != null)
            gameManager.ResetHighScore();
        PlayerPrefs.DeleteKey(HighScoreKey);
        PlayerPrefs.Save();
        UpdateHighScoreText();
    }

    void UpdateHighScoreText()
    {
        if (highScoreText != null)
            highScoreText.text = "HIGHSCORE: " + PlayerPrefs.GetInt(HighScoreKey, 0);
    }
}
