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

    public override void Awake()
    {
        base.Awake();
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

    public void SetScore(int score)
    {
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
