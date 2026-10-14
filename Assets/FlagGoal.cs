using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class FlagGoal : MonoBehaviour
{
    public AudioClip levelCompleteSound; 
    private AudioSource flagAudio;
    private bool reached = false;

    void Start()
    {
        flagAudio = GetComponent<AudioSource>();
        GameManager.instance.gameRestart.AddListener(GameRestart);
    }

    void OnDestroy()
    {
        if (GameManager.instance != null)
            GameManager.instance.gameRestart.RemoveListener(GameRestart);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (reached || !other.CompareTag("Player"))
            return;
        PlayerMovement mario = other.GetComponent<PlayerMovement>();
        if (mario == null || !mario.alive)
            return;

        reached = true;

        if (levelCompleteSound != null)
            flagAudio.PlayOneShot(levelCompleteSound);
        mario.ShowEndScreen("You Win!");
    }

    void GameRestart()
    {
        reached = false;
        flagAudio.Stop();
    }
}
