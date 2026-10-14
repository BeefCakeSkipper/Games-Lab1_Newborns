using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
public class GameManager : Singleton<GameManager>
{
    public GameConstants gameConstants;
    public UnityEvent gameStart;
    public UnityEvent gameRestart;
    public UnityEvent<int> scoreChange;
    public UnityEvent gameOver;

    private int score = 0;

    // best score ever, saved between play sessions
    private const string HighScoreKey = "HighScore"; // MainMenu reads/resets the same key
    public int highScore { get; private set; }

    public override void Awake()
    {
        base.Awake();
        highScore = PlayerPrefs.GetInt(HighScoreKey, 0);
        if (gameConstants == null)
        {
            gameConstants = ScriptableObject.CreateInstance<GameConstants>();
        }
    }

    void Start()
    {
        score = gameConstants.startingScore;
        gameStart.Invoke();
        Time.timeScale = 1.0f;
        // subscribe to scene manager scene change
        SceneManager.activeSceneChanged += SceneSetup;
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void SceneSetup(Scene current, Scene next)
    {
        gameStart.Invoke();
        SetScore(score);
    }
    public void GameRestart()
    {
        // reset score
        score = gameConstants.startingScore;
        SetScore(score);
        gameRestart.Invoke();
        Time.timeScale = 1.0f;
    }

    public void IncreaseScore(int increment)
    {
        score += increment;
        SetScore(score);
    }

    // called by the main menu's reset highscore button
    public void ResetHighScore()
    {
        highScore = 0;
        PlayerPrefs.DeleteKey(HighScoreKey);
        PlayerPrefs.Save();
    }

    public void SetScore(int score)
    {
        // save a new high score before telling the HUD, so it shows the updated one
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt(HighScoreKey, highScore);
            PlayerPrefs.Save();
        }
        scoreChange.Invoke(score);
    }


    public void GameOver()
    {
        Time.timeScale = 0.0f;
        gameOver.Invoke();
    }

    protected override void OnDestroy()
    {
        SceneManager.activeSceneChanged -= SceneSetup;
        base.OnDestroy();
    }
}
