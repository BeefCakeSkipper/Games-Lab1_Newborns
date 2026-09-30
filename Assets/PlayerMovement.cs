using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 10;
    private Rigidbody2D marioBody;

    public float upSpeed = 2;
    public GameObject enemies;
    public TextMeshProUGUI scoreText;
    public CameraFollow cameraFollow;
    public BossLogic boss;
    public int score = 0;
    public BossTrigger bossTrigger;
    public GameObject gameOverScreen;
    public TextMeshProUGUI resultText;
    private bool onGroundState = true;
    private SpriteRenderer marioSprite;
    private bool faceRightState = true;
    private Vector3 startPosition; // taking it from the scene start
    public bool isOnGround => onGroundState;
    public Vector3 boxSize;
    public float maxDistance;
    public LayerMask layerMask;
    public Sprite jumpSprite;
    private Sprite normalSprite;
    public Sprite[] walkSprites;
    public float walkFrameTime = 0.12f;   // seconds each frame is held
    private float walkTimer;
    private int walkFrame;
    public Animator marioAnimator;
    public AudioSource marioAudio;
    public AudioClip marioDeath;
    public float deathImpulse = 15;

    // state
    [System.NonSerialized]
    public bool alive = true;

    void PlayDeathImpulse()
    {
        marioBody.AddForce(Vector2.up * deathImpulse, ForceMode2D.Impulse);
    }

    void PlayJumpSound()
    {
        marioAudio.PlayOneShot(marioAudio.clip);
    }


    // Start is called before the first frame update
    void Start()
    {
        // Set to be 30 FPS
        Application.targetFrameRate = 30;
        marioBody = GetComponent<Rigidbody2D>();
        marioSprite = GetComponent<SpriteRenderer>();
        normalSprite = marioSprite.sprite;
        startPosition = transform.localPosition;
        // ground check hits layer 3 (Ground) and layer 7 (Obstacles)
        layerMask = (1 << 3) | (1 << 7);
        // scoreValue = GetComponent<JumpOnGoomba>();

        // update animator state
        // marioAnimator.SetBool("onGround", onGroundState);
    }

    // Update is called once per frame
    void Update()
    {
        // always check if mario is on the ground so if u fall on the goomba it still gets stomped
        onGroundState = onGroundCheck();
        marioAnimator.SetBool("onGround", onGroundState);
        if (Input.GetKeyDown("a") && faceRightState)
        {
            faceRightState = false;
            marioSprite.flipX = true;
            // if (marioBody.linearVelocity.x > 0.1f)
            marioAnimator.SetTrigger("onSkid");
        }

        if (Input.GetKeyDown("d") && !faceRightState)
        {
            faceRightState = true;
            marioSprite.flipX = false;
            // if (marioBody.linearVelocity.x < -0.1f)
            marioAnimator.SetTrigger("onSkid");
        }

        if (Input.GetKeyDown("space") && onGroundState)
        {
            marioBody.AddForce(Vector2.up * upSpeed, ForceMode2D.Impulse);
        }

        marioAnimator.SetFloat("xSpeed", Mathf.Abs(marioBody.linearVelocity.x));

        UpdateSprite();

    }
    public float maxSpeed = 20;
    // FixedUpdate is called 50 times a second
    void FixedUpdate()
    {
        if (alive)
        {
            float moveHorizontal = Input.GetAxisRaw("Horizontal");

            if (Mathf.Abs(moveHorizontal) > 0)
            {
                Vector2 movement = new Vector2(moveHorizontal, 0);
                // check if it doesn't go beyond maxSpeed
                if (marioBody.linearVelocity.x < maxSpeed)
                    marioBody.AddForce(movement * speed);
            }

            if (Input.GetKeyDown("space") && onGroundState)
            {
                marioBody.AddForce(Vector2.up * upSpeed, ForceMode2D.Impulse);
                onGroundState = false;
                // update animator state
                marioAnimator.SetBool("onGround", onGroundState);
            }

            // stop
            if (Input.GetKeyUp("a") || Input.GetKeyUp("d"))
            {
                // stop
                marioBody.linearVelocity = new Vector2(0, marioBody.linearVelocity.y);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground") && !onGroundState)
        {
            onGroundState = true;
            // update animator state
            marioAnimator.SetBool("onGround", onGroundState);
        }

        // grounding is handled by onGroundCheck() now, not by collisions
        if (alive && col.gameObject.CompareTag("Enemy")
            && transform.position.y - col.transform.position.y < 0.4f)
        {
            // play death animation
            marioAnimator.Play("mario-die");
            marioAudio.PlayOneShot(marioDeath);
            alive = false;
            ShowEndScreen("GAME OVER");
        }
        if (col.gameObject.CompareTag("Boss"))
        {
            ContactPoint2D contact = col.GetContact(0);
            if (contact.normal.y < 0.7f)
            {
                ShowEndScreen("GAME OVER");
            }
        }
    }

    // sprite helper for animations
    private void UpdateSprite()
    {
        marioSprite.flipX = !faceRightState;

        if (!onGroundState)
        {
            marioSprite.sprite = jumpSprite;
            return;
        }

        if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0 && walkSprites.Length > 0)
        {
            walkTimer += Time.deltaTime;
            if (walkTimer >= walkFrameTime)
            {
                walkTimer -= walkFrameTime;
                walkFrame = (walkFrame + 1) % walkSprites.Length;
            }
            marioSprite.sprite = walkSprites[walkFrame];
        }
        else
        {
            marioSprite.sprite = normalSprite;
            walkTimer = 0.0f;
            walkFrame = 0;
        }
    }

    private bool onGroundCheck()
    {
        if (Physics2D.BoxCast(transform.position, boxSize, 0, -transform.up, maxDistance, layerMask))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(transform.position - transform.up * maxDistance, boxSize);
    }

    // shows the end screen
    public void ShowEndScreen(string message)
    {
        resultText.text = message;
        gameOverScreen.SetActive(true);
        bossTrigger.bossHealth.SetActive(false);
        Time.timeScale = 0.0f;
    }

    public void RestartButtonCallback(int input)
    {
        gameOverScreen.SetActive(false);
        // reset everything
        ResetGame();
        cameraFollow.enabled = true;

        // resume time
        Time.timeScale = 1.0f;
    }

    private void ResetGame()
    {
        // reset position
        marioBody.transform.localPosition = startPosition;
        marioBody.linearVelocity = Vector2.zero;
        // reset sprite direction
        faceRightState = true;
        // reset score
        score = 0;
        scoreText.text = "Score: 0";
        // reset Goomba
        foreach (Transform eachChild in enemies.transform)
        {
            eachChild.GetComponent<EnemyMovement>().Respawn();
        }
        // reset boss
        boss.Respawn();
        bossTrigger.ResetTrigger();

        // reset animation
        marioAnimator.SetTrigger("gameRestart");
        alive = true;

    }
    public void AddScore()
    {
        score++;
        scoreText.text = "Score: " + score.ToString();
    }



}