using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerMovement : Singleton<PlayerMovement>
{
    public float speed = 10;
    private Rigidbody2D marioBody;

    public float upSpeed = 2;
    public CameraFollow cameraFollow;
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
    public AudioSource marioDeathAudio;
    public AudioClip marioDeath;
    public AudioClip marioJump;
    public AudioSource LevelCompleteAudio;
    public float deathImpulse = 15;
    public bool isDead = false;

    // state
    [System.NonSerialized]
    public bool alive = true;

    public float deathDelay = 3.0f;   // seconds before the game over screen shows

    // magic mushroom
    public RuntimeAnimatorController bigMarioController; // override controller with the big mario sprites
    public Vector2 bigColliderSize = new Vector2(1, 2);   // big mario's hitbox
    public Vector2 bigColliderOffset = new Vector2(0, 0.5f);
    public float bigMaxDistance = 0.1f;                   // ground check distance for big mario
    public float hurtInvincibleTime = 1.5f; // seconds mario can't be hit after shrinking
    public AudioClip marioPowerup;
    public AudioClip marioPowerdown;
    public int sizeFlickers = 8;            // how many times mario flickers between small and big
    public float flickerInterval = 0.08f;   // seconds between each flicker
    private Coroutine sizeAnimation;
    private bool isBig = false;
    private bool invincible = false;
    private RuntimeAnimatorController smallMarioController;
    private BoxCollider2D marioCollider;
    private Vector2 smallColliderSize;
    private Vector2 smallColliderOffset;

    // starman
    public float starDuration = 10f;        // seconds of invincibility
    public float starColorSpeed = 10f;      // how fast mario cycles colours
    public AudioSource starMusic;           // plays the starman theme while invincible
    public AudioClip starKillSound;         // when mario runs into an enemy while invincible
    private bool starPower = false;
    private Coroutine starAnimation;
    private AudioSource pausedMusic;        // level/boss music paused while the star music plays
    // Property
    public override void Awake()
    {
        Debug.Log("Mario Awake called");
        Debug.Log($"Mario Awake: {gameObject.name}");
        Debug.Log($"Mario Instance: {PlayerMovement.instance}");
        base.Awake();

        Debug.Log($"Mario scene: {gameObject.scene.name}");
        GameManager.instance.gameRestart.AddListener(GameRestart);
        GameManager.instance.gameOver.AddListener(GameOver);
    }
    void PlayDeathImpulse()
    {
        marioBody.linearVelocity = Vector2.zero;
        marioBody.AddForce(Vector2.up * deathImpulse, ForceMode2D.Impulse);
    }

    public void GameRestart()
    {
        // the HUDManager hides the game over screen itself on restart
        ResetGame();
        if (cameraFollow != null)
            cameraFollow.enabled = true;
    }


    void GameOver()
    {
        ShowEndScreen("GAME OVER");
    }

    // kills mario dedge
    public void Die()
    {
        if (!alive || invincible || starPower)
            return;

        // big mario shrinks instead of dying
        if (isBig)
        {
            PowerDown();
            return;
        }

        isDead = true;
        alive = false;

        marioAnimator.Play("mario-die");

        if (marioDeathAudio != null && marioDeath != null)
        {
            marioDeathAudio.PlayOneShot(marioDeath);
        }
        StopAllMusic();

        GetComponent<Collider2D>().enabled = false;

        Invoke(nameof(GameOver), deathDelay);
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
        // remember small mario's animator and hitbox
        smallMarioController = marioAnimator.runtimeAnimatorController;
        marioCollider = GetComponent<BoxCollider2D>();
        smallColliderSize = marioCollider.size;
        smallColliderOffset = marioCollider.offset;
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

    // if invincibility ends and mario is still touching enemy nuke him
    void OnCollisionStay2D(Collision2D col)
    {
        if (!alive)
            return;

        if (col.gameObject.CompareTag("Enemy")
            && transform.position.y - col.transform.position.y < 0.4f)
        {
            if (starPower)
                StarKill(col.gameObject.GetComponent<EnemyMovement>());
            else if (!invincible)
                Die();
        }
        else if (col.gameObject.CompareTag("Boss") && !invincible && !starPower)
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
            // FixedUpdate slows mario down from here
        }
        else
        {
            FlipMarioSprite(value);
            moving = true;
            Move(value);
        }
    }
    // called by the magic mushroom
    public void MakeBig()
    {
        if (isBig)
            return;
        isBig = true;
        if (marioAudio != null && marioPowerup != null)
            marioAudio.PlayOneShot(marioPowerup);
        // taller hitbox straight away, sprites flicker into big mario
        SetBigCollider(true);
        StartSizeAnimation(GrowAnimation());
    }

    // big mario got hit, shrink and become invincible for a bit
    void PowerDown()
    {
        isBig = false;
        invincible = true;
        if (marioAudio != null && marioPowerdown != null)
            marioAudio.PlayOneShot(marioPowerdown);
        SetBigCollider(false);
        StartSizeAnimation(ShrinkAnimation());
    }

    // called by the starman powerup
    public void MakeStar()
    {
        if (starAnimation != null)
            StopCoroutine(starAnimation); // picking up another star restarts the timer
        else
            StartStarMusic();
        starPower = true;
        starAnimation = StartCoroutine(StarAnimation());
    }

    // cycle mario's colour until the star runs out
    IEnumerator StarAnimation()
    {
        float elapsed = 0;
        while (elapsed < starDuration)
        {
            marioSprite.color = Color.HSVToRGB((elapsed * starColorSpeed) % 1f, 0.6f, 1f);
            yield return null;
            elapsed += Time.deltaTime;
        }
        EndStar(true);
    }

    void EndStar(bool resumeMusic)
    {
        if (starAnimation != null)
            StopCoroutine(starAnimation);
        starAnimation = null;
        starPower = false;
        marioSprite.color = Color.white;

        if (starMusic != null)
            starMusic.Stop();
        if (resumeMusic && pausedMusic != null)
            pausedMusic.UnPause();
        pausedMusic = null;
    }

    // called when the level switches music so the star music has priority
    public void MusicChanged(AudioSource newMusic)
    {
        if (!starPower || newMusic == null)
            return;
        newMusic.Pause();
        pausedMusic = newMusic; // resume this one when the star runs out
    }

    // stop the level, boss and star music (restart plays the level music again)
    void StopAllMusic()
    {
        EndStar(false);
        if (bossTrigger != null)
        {
            if (bossTrigger.backgroundMusic != null)
                bossTrigger.backgroundMusic.Stop();
            if (bossTrigger.bossMusic != null)
                bossTrigger.bossMusic.Stop();
        }
    }

    void StartStarMusic()
    {
        // pause whatever is playing and play the star theme
        pausedMusic = null;
        if (bossTrigger != null)
        {
            if (bossTrigger.bossMusic != null && bossTrigger.bossMusic.isPlaying)
                pausedMusic = bossTrigger.bossMusic;
            else if (bossTrigger.backgroundMusic != null && bossTrigger.backgroundMusic.isPlaying)
                pausedMusic = bossTrigger.backgroundMusic;
        }
        if (pausedMusic != null)
            pausedMusic.Pause();
        if (starMusic != null)
            starMusic.Play();
    }

    void StarKill(EnemyMovement enemy)
    {
        if (enemy == null)
            return;
        enemy.Squash();
        GameManager.instance.IncreaseScore(GameManager.instance.gameConstants.starKillScore);
        if (marioAudio != null && starKillSound != null)
            marioAudio.PlayOneShot(starKillSound);
    }

    // instantly back to small mario, no sound or animation (used on restart)
    void MakeSmall()
    {
        StopSizeAnimation();
        isBig = false;
        invincible = false;
        SetBigSprites(false);
        SetBigCollider(false);
    }

    void SetBigSprites(bool big)
    {
        if (big && bigMarioController != null)
            marioAnimator.runtimeAnimatorController = bigMarioController;
        else
            marioAnimator.runtimeAnimatorController = smallMarioController;
    }

    void SetBigCollider(bool big)
    {
        marioCollider.size = big ? bigColliderSize : smallColliderSize;
        marioCollider.offset = big ? bigColliderOffset : smallColliderOffset;
    }

    // doing this cause idk how to make him invincible while the animation plays
    void StartSizeAnimation(IEnumerator animation)
    {
        StopSizeAnimation();
        sizeAnimation = StartCoroutine(animation);
    }

    void StopSizeAnimation()
    {
        if (sizeAnimation != null)
            StopCoroutine(sizeAnimation);
        sizeAnimation = null;
        marioSprite.enabled = true;
    }

    IEnumerator GrowAnimation()
    {
        for (int i = 0; i < sizeFlickers; i++)
        {
            SetBigSprites(i % 2 == 1);
            yield return new WaitForSeconds(flickerInterval);
        }
        SetBigSprites(true);
        sizeAnimation = null;
    }

    IEnumerator ShrinkAnimation()
    {
        float elapsed = 0;
        for (int i = 0; i < sizeFlickers; i++)
        {
            SetBigSprites(i % 2 == 1);
            yield return new WaitForSeconds(flickerInterval);
            elapsed += flickerInterval;
        }
        SetBigSprites(false);

        while (elapsed < hurtInvincibleTime)
        {
            marioSprite.enabled = !marioSprite.enabled;
            yield return new WaitForSeconds(flickerInterval);
            elapsed += flickerInterval;
        }
        marioSprite.enabled = true;
        invincible = false;
        sizeAnimation = null;
    }

    private bool onGroundCheck()
    {
        // big mario's feet are further from his center, so check further down
        float distance = isBig ? bigMaxDistance : maxDistance;
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
        // the HUD only exists if the game was started from a level that has one
        if (HUDManager.instance != null)
            HUDManager.instance.ShowEndScreen(message);
        EndStar(false);
        // not every level has a boss
        if (bossTrigger != null)
            bossTrigger.bossHealth.SetActive(false);

        Time.timeScale = 0.0f;
    }

    private void ResetGame()
    {
        isDead = false;
        Time.timeScale = 1.0f;
        AudioListener.pause = false;
        // reset position
        marioBody.transform.localPosition = startPosition;
        marioBody.linearVelocity = Vector2.zero;
        // reset sprite direction
        faceRightState = true;
        marioSprite.flipX = false;
        moving = false;
        jumpedState = false;
        // back to small mario
        MakeSmall();
        // lose the star (BossTrigger restarts the level music on restart)
        EndStar(false);
        // the score and the HUD are reset by GameManager and HUDManager
        // goombas, blocks/powerups and the boss + music reset themselves on GameManager.gameRestart

        if (LevelCompleteAudio != null)
            LevelCompleteAudio.Stop();

        // reset animation
        marioAnimator.SetTrigger("gameRestart");
        alive = true;
        // give mario his hitbox back
        GetComponent<Collider2D>().enabled = true;


    }
    // called when mario stomps the boss, goes through the GameManager so the score stays in one place
    public void AddScore()
    {
        GameManager.instance.IncreaseScore(GameManager.instance.gameConstants.bossHitScore);
    }

    public void SetStartingPosition(Scene current, Scene next)
    {

        if (next.name == "SampleScene" || next.name.StartsWith("World"))
        {
            // change the position accordingly in your World-1-2 case
            this.transform.position = GameManager.instance.gameConstants.marioStartingPosition;
            // restarting in this level should put mario back here, not at the previous level's start
            startPosition = transform.localPosition;
        }
        FindSceneReferences();
    }

    // mario survives scene changes but the camera and boss trigger don't, so grab the new scene's ones
    void FindSceneReferences()
    {
        cameraFollow = FindFirstObjectByType<CameraFollow>();
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

    protected override void OnDestroy()
    {
        // Unsubscribe from scene change events
        SceneManager.activeSceneChanged -= SetStartingPosition;
        // stop listening to the GameManager (it survives going back to the main menu, mario doesn't)
        if (GameManager.instance != null)
        {
            GameManager.instance.gameRestart.RemoveListener(GameRestart);
            GameManager.instance.gameOver.RemoveListener(GameOver);
        }
        Debug.Log($"Mario Destroyed: {gameObject.name}");
        // Clear singleton reference
        base.OnDestroy();
    }
}




