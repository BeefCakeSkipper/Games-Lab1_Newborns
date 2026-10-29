using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    [Header("ScriptableObject References")]
    [SerializeField] private GameConstants gameConstants;
    [SerializeField] private IntGameEvent onIncrementScore;
    [Header("Boss UI")]
    [SerializeField] private GameObject bossHealthUI;
    public float speed = 10;
    private Rigidbody2D marioBody;

    public float upSpeed = 2;
    public CameraController CameraController;
    public BossTrigger bossTrigger;
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
    public AudioClip marioJump;
    public AudioSource LevelCompleteAudio;
    public float deathImpulse = 15;

    // state
    private MarioStateController marioStateController;
    public bool isDead => marioStateController.isDead;
    public bool alive => !marioStateController.isDead;

    public float bigMaxDistance = 0.1f;                   // ground check distance for big mario
    public bool isLevelComplete = false;
    // Property
    private void Awake()
    {
        marioStateController = GetComponent<MarioStateController>();
        Debug.Log("Mario Awake called");
        Debug.Log($"Mario Awake: {gameObject.name}");
        Debug.Log($"Mario scene: {gameObject.scene.name}");
    }
    void PlayDeathImpulse()
    {
        marioBody.linearVelocity = Vector2.zero;
        marioBody.AddForce(Vector2.up * deathImpulse, ForceMode2D.Impulse);
    }

    public void GameRestart()
    {
        ResetGame();
    }


    public void GameOver()
    {
        ShowEndScreen("GAME OVER");
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log($"Mario is in scene: {gameObject.scene.name}");
        // Set to be 30 FPS
        SceneManager.activeSceneChanged += SetStartingPosition;
        Application.targetFrameRate = 30;
        marioBody = GetComponent<Rigidbody2D>();
        marioSprite = GetComponent<SpriteRenderer>();
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
        marioAnimator.SetFloat("xSpeed", Mathf.Abs(marioBody.linearVelocity.x));
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Debug.Log($"TimeScale: {Time.timeScale}");
            Debug.Log($"Mario alive: {alive}");
            Debug.Log($"Mario isDead: {isDead}");
        }
    }
    public float maxSpeed = 20;
    public float stopDamping = 0.7f; // how fast mario slows down after letting go
    // FixedUpdate is called 50 times a second
    void FixedUpdate()
    {
        if (alive && moving)
        {
            Move(faceRightState == true ? 1 : -1);
        }
        else if (alive)
        {
            // skid
            marioBody.linearVelocity = new Vector2(marioBody.linearVelocity.x * stopDamping, marioBody.linearVelocity.y);
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

        // Check horizontal speed instead of total velocity
        if (Mathf.Abs(marioBody.linearVelocity.x) < maxSpeed)
        {
            marioBody.AddForce(movement * speed);
        }
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
            // FixedUpdate slows mario down from here
        }
        else
        {
            FlipMarioSprite(value);
            moving = true;
            Move(value);
        }
    }

    // called when the level switches music so the star music has priority
    public void MusicChanged(AudioSource newMusic)
    {
        marioStateController.MusicChanged(newMusic);
    }

    private bool onGroundCheck()
    {
        // big mario's feet are further from his center, so check further down
        float distance = marioStateController.isBig ? bigMaxDistance : maxDistance;
        if (Physics2D.BoxCast(transform.position, boxSize, 0, -transform.up, distance, layerMask))
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
        // HUDManager listens to OnGameOver.asset
        // and displays the game-over screen itself.
        if (message == "You Win!")
        {
            isLevelComplete = true;
        }
        marioStateController.EndStar(false);
        if (bossTrigger != null)
            bossTrigger.HideBossHealthUI();


        Time.timeScale = 0.0f;
    }

    private void ResetGame()
    {
        Time.timeScale = 1.0f;
        AudioListener.pause = false;

        // Restore Mario's physics.
        marioBody.simulated = true;
        marioBody.linearVelocity = Vector2.zero;
        marioBody.angularVelocity = 0f;

        // Reset Mario's position.
        marioBody.transform.localPosition = startPosition;
        // reset sprite direction
        faceRightState = true;
        marioSprite.flipX = false;
        moving = false;
        jumpedState = false;
        // size, star, invincibility and death are reset by MarioStateController.GameRestart
        // the score and the HUD are reset by GameManager and HUDManager
        // goombas, blocks/powerups and the boss + music reset themselves on GameManager.gameRestart

        if (LevelCompleteAudio != null)
            LevelCompleteAudio.Stop();

        // reset animation
        marioAnimator.SetTrigger("gameRestart");


    }
    // called when mario stomps the boss, goes through the GameManager so the score stays in one place
    public void AddScore()
    {
        onIncrementScore.Raise(gameConstants.bossHitScore);
    }

    public void SetStartingPosition(Scene current, Scene next)
    {
        if (next.name == "SampleScene" || next.name.StartsWith("World"))
        {
            transform.position = gameConstants.marioStartingPosition;
            startPosition = transform.localPosition;
        }

        // Reconnect references belonging to the newly loaded scene.
        FindSceneReferences();

        if (CameraController != null)
        {
            CameraController.enabled = true;
        }
    }

    // mario survives scene changes but the camera and boss trigger don't, so grab the new scene's ones
    void FindSceneReferences()
    {
        CameraController = FindFirstObjectByType<CameraController>();
        bossTrigger = FindFirstObjectByType<BossTrigger>(); // null in levels without a boss
    }

    // Allows player to move to next scene
    public void NextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("Already at the last scene!");
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from scene change events
        SceneManager.activeSceneChanged -= SetStartingPosition;
        // stop listening to the GameManager (it survives going back to the main menu, mario doesn't)

        Debug.Log($"Mario Destroyed: {gameObject.name}");
    }
}
