using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class JumpOnGoomba : MonoBehaviour
{
    // public TextMeshProUGUI scoreText;

    // [System.NonSerialized]
    // public int score = 0; // we don't want this to show up in the inspector

    public float bounceSpeed = 5.0f;   // lil bump after stomp
    private Rigidbody2D marioBody;
    private PlayerMovement playerMovement;
    public AudioClip stompSound;

    public delegate void GoombaStompHandler(EnemyMovement goomba);
    public event GoombaStompHandler goombaStomped;
    public GameManagerWeek3 gameManager;



    // Start is called before the first frame update
    void Start()
    {
        marioBody = GetComponent<Rigidbody2D>();
        playerMovement = GetComponent<PlayerMovement>();

        goombaStomped += AddStompScore;
        goombaStomped += SquashGoomba;
        goombaStomped += PlayStompSound;
        goombaStomped += Bounce;
    }

    void OnDestroy()
    {
        goombaStomped -= AddStompScore;
        goombaStomped -= SquashGoomba;
        goombaStomped -= PlayStompSound;
        goombaStomped -= Bounce;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        // mario stomps the goomba: airborne from a jump, haven't scored yet this
        // jump, and mario is above the goomba (so side hits don't count)
        if (col.gameObject.CompareTag("Enemy") && !playerMovement.isOnGround
            && transform.position.y > col.transform.position.y + 0.4f)
        {
            // score++;
            // scoreText.text = "Score: " + score.ToString();
            goombaStomped?.Invoke(col.gameObject.GetComponent<EnemyMovement>());
        }
    }

    void AddStompScore(EnemyMovement goomba)
    {
        gameManager.IncreaseScore(1);
    }

    void SquashGoomba(EnemyMovement goomba)
    {
        goomba.Squash();
    }

    void PlayStompSound(EnemyMovement goomba)
    {
        playerMovement.marioAudio.PlayOneShot(stompSound);
    }

    void Bounce(EnemyMovement goomba)
    {
        marioBody.AddForce(Vector2.up * bounceSpeed, ForceMode2D.Impulse);
    }


}
