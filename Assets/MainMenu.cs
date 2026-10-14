using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public TextMeshProUGUI highScoreText;
    private const string HighScoreKey = "HighScore"; // same key PlayerMovement saves the high score under

    void Start()
    {
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
        if (GameManager.instance != null)
            GameManager.instance.ResetHighScore();
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
