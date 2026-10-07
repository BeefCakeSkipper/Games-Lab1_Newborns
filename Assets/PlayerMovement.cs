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
    private bool moving = false;
    private bool jumpedState = false;
    private Vector3 startPosition; // taking it from the scene start
    public bool isOnGround => onGroundState;
    public Vector3 boxSize;
    public float maxDistance;
    public LayerMask layerMask;
    public Animator marioAnimator;
    public AudioSource marioAudio;
    public AudioSource marioDeathAudio;
    public AudioClip marioDeath;
    public AudioClip marioJump;
    public AudioSource LevelCompleteAudio;
    public float deathImpulse = 15;
    public GameObject obstacles;
    public Vector2 gameOverScorePosition = new Vector2(0, -80); // below the game over text

    // original score position
    private Vector2 scoreAnchorMin;
    private Vector2 scoreAnchorMax;
    private Vector2 scorePosition;
    private TextAlignmentOptions scoreAlignment;
    private int scoreSiblingIndex;


    // state
    [System.NonSerialized]
    public bool alive = true;

    public float deathDelay = 3.0f;   // seconds before the game over screen shows

    void PlayDeathImpulse()
    {
        marioBody.linearVelocity = Vector2.zero;
        marioBody.AddForce(Vector2.up * deathImpulse, ForceMode2D.Impulse);
    }

    void GameOver()
    {
        ShowEndScreen("GAME OVER");
    }

    // kills mario dedge
    public void Die()
    {
        if (!alive)
            return;

        // play death animation
        marioAnimator.Play("mario-die");
        // marioAudio.PlayOneShot(marioDeath); // Removed this to use marioDeathAudio instead which is a separate AudioSource for the death sound
        marioDeathAudio.PlayOneShot(marioDeath);
        alive = false;
        // remove hitbox so mario falls through everything
        GetComponent<Collider2D>().enabled = false;
        // wait for the death impulse + animation before freezing the game
        Invoke(nameof(GameOver), deathDelay);
    }

    // Start is called before the first frame update
    void Start()
    {
        // Set to be 30 FPS
        Application.targetFrameRate = 30;
        marioBody = GetComponent<Rigidbody2D>();
        marioSprite = GetComponent<SpriteRenderer>();
        startPosition = transform.localPosition;
        // ground check hits layer 3 (Ground) and layer 7 (Obstacles)
        layerMask = (1 << 3) | (1 << 7);

        // remember where the score sits in the HUD
        RectTransform scoreRect = scoreText.rectTransform;
        scoreAnchorMin = scoreRect.anchorMin;
        scoreAnchorMax = scoreRect.anchorMax;
        scorePosition = scoreRect.anchoredPosition;
        scoreAlignment = scoreText.alignment;
        scoreSiblingIndex = scoreRect.GetSiblingIndex();
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
        marioAnimator.SetFloat("xSpeed", Mathf.Abs(marioBody.linearVelocity.x));
    }
    public float maxSpeed = 20;
    // FixedUpdate is called 50 times a second
    void FixedUpdate()
    {
        if (alive && moving)
        {
            Move(faceRightState == true ? 1 : -1);
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        // grounding is handled by onGroundCheck() now, not by collisions
        if (alive && col.gameObject.CompareTag("Enemy")
            && transform.position.y - col.transform.position.y < 0.4f)
        {
            Die();
        }
        if (col.gameObject.CompareTag("Boss"))
        {
            ContactPoint2D contact = col.GetContact(0);
            if (contact.normal.y < 0.7f)
            {
                Die();
            }
        }
    }

    // sprite helper for animations
    void FlipMarioSprite(int value)
    {
        if (value == -1 && faceRightState)
        {
            faceRightState = false;
            marioSprite.flipX = true;
            if (marioBody.linearVelocity.x > 0.05f)
                marioAnimator.SetTrigger("onSkid");

        }

        else if (value == 1 && !faceRightState)
        {
            faceRightState = true;
            marioSprite.flipX = false;
            if (marioBody.linearVelocity.x < -0.05f)
                marioAnimator.SetTrigger("onSkid");
        }
    }

    void Move(int value)
    {

        Vector2 movement = new Vector2(value, 0);
        // check if it doesn't go beyond maxSpeed
        if (marioBody.linearVelocity.magnitude < maxSpeed)
            marioBody.AddForce(movement * speed);
    }
    public void Jump()
    {
        if (alive && onGroundState)
        {
            // jump
            marioBody.AddForce(Vector2.up * upSpeed, ForceMode2D.Impulse);
            marioAudio.PlayOneShot(marioJump);
            onGroundState = false;
            jumpedState = true;
            // update animator state
            marioAnimator.SetBool("onGround", onGroundState);

        }
    }
    public void JumpHold()
    {
        if (alive && jumpedState)
        {
            // jump higher
            marioBody.AddForce(Vector2.up * upSpeed * 30, ForceMode2D.Force);
            jumpedState = false;

        }
    }

    public void MoveCheck(int value)
    {
        if (value == 0)
        {
            moving = false;
            marioBody.linearVelocity = new Vector2(0, marioBody.linearVelocity.y);
        }
        else
        {
            FlipMarioSprite(value);
            moving = true;
            Move(value);
        }
    }
    public void SetScore(int newScore)
    {
        score = newScore;
        scoreText.text = "Score: " + score.ToString();
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

        // move the score to the center and below the game over text
        RectTransform scoreRect = scoreText.rectTransform;
        scoreRect.anchorMin = new Vector2(0.5f, 0.5f);
        scoreRect.anchorMax = new Vector2(0.5f, 0.5f);
        scoreRect.anchoredPosition = gameOverScorePosition;
        scoreText.alignment = TextAlignmentOptions.Center;
        scoreRect.SetAsLastSibling(); // put score over the gameover overlay

        Time.timeScale = 0.0f;
    }

    public void RestartButtonCallback(int input)
    {
        gameOverScreen.SetActive(false);
        // reset everything
        ResetGame();
        cameraFollow.enabled = true;

        // resume time
        // Time.timeScale = 1.0f;
    }

    private void ResetGame()
    {
        // reset position
        marioBody.transform.localPosition = startPosition;
        marioBody.linearVelocity = Vector2.zero;
        // reset sprite direction
        faceRightState = true;
        marioSprite.flipX = false;
        moving = false;
        jumpedState = false;
        // reset score
        score = 0;
        scoreText.text = "Score: 0";
        // put the score back to the corner
        RectTransform scoreRect = scoreText.rectTransform;
        scoreRect.anchorMin = scoreAnchorMin;
        scoreRect.anchorMax = scoreAnchorMax;
        scoreRect.anchoredPosition = scorePosition;
        scoreText.alignment = scoreAlignment;
        scoreRect.SetSiblingIndex(scoreSiblingIndex);
        // reset Goomba
        foreach (Transform eachChild in enemies.transform)
        {
            eachChild.GetComponent<EnemyMovement>().Respawn();
        }
        // reset blocks
        foreach (MarioBlock block in obstacles.GetComponentsInChildren<MarioBlock>())
        {
            block.ResetBlock();
        }
        // reset boss
        boss.Respawn();
        bossTrigger.ResetTrigger();
        LevelCompleteAudio.Stop();

        // reset animation
        marioAnimator.SetTrigger("gameRestart");
        alive = true;
        // give mario his hitbox back
        GetComponent<Collider2D>().enabled = true;
        Time.timeScale = 1.0f;

    }
    public void AddScore()
    {
        score++;
        scoreText.text = "Score: " + score.ToString();
    }



}