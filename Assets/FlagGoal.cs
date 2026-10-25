using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class FlagGoal : MonoBehaviour
{
    [Header("Game Events")]
    [SerializeField] private SimpleGameEvent onLevelComplete;
    public AudioClip levelCompleteSound;
    private AudioSource flagAudio;
    private bool reached = false;

    void Start()
    {
        flagAudio = GetComponent<AudioSource>();
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
        if (onLevelComplete != null)
            onLevelComplete.Raise();
    }

    public void GameRestart()
    {
        reached = false;
        flagAudio.Stop();
    }
}
