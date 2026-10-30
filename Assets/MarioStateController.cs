using System;
using System.Collections;
using UnityEngine;

public class MarioStateController : StateController
{
    public GameConstants gameConstants;
    public PowerupType currentPowerupType = PowerupType.Default;
    public MarioState shouldBeNextState = MarioState.Default;
    private SpriteRenderer spriteRenderer;

    public IntGameEvent onIncrementScore;
    public SimpleGameEvent onGameOver;
    public AudioSource marioAudio;
    public AudioSource marioDeathAudio;
    public AudioSource starMusic;
    public AudioClip starKillSound;
    public Vector2 bigColliderSize = new Vector2(1, 2);
    public Vector2 bigColliderOffset = new Vector2(0, 0.5f);
    public int sizeFlickers = 8;
    public float starColorSpeed = 10f;
    public float deathDelay = 3.0f;

    [HideInInspector] public bool isBig;
    [HideInInspector] public bool invincible;
    [HideInInspector] public bool starPower;
    [HideInInspector] public bool isDead;

    private BoxCollider2D marioCollider;
    private Vector2 smallColliderSize;
    private Vector2 smallColliderOffset;
    private Coroutine sizeAnimation;
    private Coroutine starAnimation;
    private AudioSource pausedMusic;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        marioCollider = GetComponent<BoxCollider2D>();
        smallColliderSize = marioCollider.size;
        smallColliderOffset = marioCollider.offset;
    }

    public override void Start()
    {
        base.Start();
        GameRestart();
    }

    public void GameRestart()
    {
        CancelInvoke(nameof(RaiseGameOver));
        StopSizeAnimation();
        EndStar(false);
        isBig = false;
        invincible = false;
        isDead = false;
        SetBigCollider(false);
        marioCollider.enabled = true;
        currentPowerupType = PowerupType.Default;
        TransitionToState(startState);

        BuffStateController buff = GetComponent<BuffStateController>();
        if (buff != null)
            buff.GameRestart();
    }

    public void SetPowerup(PowerupType i)
    {
        currentPowerupType = i;
    }

    public void Fire()
    {
        if (isDead || currentState == null)
            return;
        currentState.DoEventTriggeredActions(this, ActionType.Attack);
    }
    public void SetRendererToFlicker()
    {
        StartCoroutine(BlinkSpriteRenderer());
    }
    private IEnumerator BlinkSpriteRenderer()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        while (string.Equals(currentState.name, "InvincibleSmallMario", StringComparison.OrdinalIgnoreCase))
        {
            // Toggle the visibility of the sprite renderer
            spriteRenderer.enabled = !spriteRenderer.enabled;

            // Wait for the specified blink interval
            yield return new WaitForSeconds(gameConstants.flickerInterval);
        }

        spriteRenderer.enabled = true;
    }

    void OnCollisionStay2D(Collision2D col)
    {
        if (isDead)
            return;

        if (col.gameObject.CompareTag("Enemy")
            && transform.position.y - col.transform.position.y < 0.4f)
        {
            if (starPower)
                StarKill(col.gameObject.GetComponent<EnemyMovement>());
            else if (!invincible)
                SetPowerup(PowerupType.Damage);
        }
        else if (col.gameObject.CompareTag("Boss") && !invincible && !starPower)
        {
            if (col.GetContact(0).normal.y < 0.7f)
                SetPowerup(PowerupType.Damage);
        }
    }

    public void MakeBig()
    {
        isBig = true;
        SetBigCollider(true);
        StopSizeAnimation();
        sizeAnimation = StartCoroutine(GrowAnimation());
    }

    public void PowerDown()
    {
        StopSizeAnimation();
        isBig = false;
        invincible = true;
        SetBigCollider(false);
    }

    public void MakeStar()
    {
        if (starAnimation != null)
            StopCoroutine(starAnimation);
        else
            StartStarMusic();
        starPower = true;
        starAnimation = StartCoroutine(StarAnimation());
    }

    public void EndStar(bool resumeMusic)
    {
        if (starAnimation != null)
            StopCoroutine(starAnimation);
        starAnimation = null;
        starPower = false;
        spriteRenderer.color = Color.white;

        if (starMusic != null)
            starMusic.Stop();
        if (resumeMusic && pausedMusic != null)
            pausedMusic.UnPause();
        pausedMusic = null;
    }

    public void MusicChanged(AudioSource newMusic)
    {
        if (!starPower || newMusic == null)
            return;
        newMusic.Pause();
        pausedMusic = newMusic;
    }

    public void KillMario()
    {
        isDead = true;
        GetComponent<Animator>().Play("mario-die");
        StopAllMusic();
        marioCollider.enabled = false;
        Invoke(nameof(RaiseGameOver), deathDelay);
    }

    IEnumerator StarAnimation()
    {
        float elapsed = 0;
        while (true)
        {
            spriteRenderer.color = Color.HSVToRGB((elapsed * starColorSpeed) % 1f, 0.6f, 1f);
            yield return null;
            elapsed += Time.deltaTime;
        }
    }

    void StartStarMusic()
    {
        pausedMusic = null;
        BossTrigger bossTrigger = FindFirstObjectByType<BossTrigger>();
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

    void StopAllMusic()
    {
        EndStar(false);
        BossTrigger bossTrigger = FindFirstObjectByType<BossTrigger>();
        if (bossTrigger != null)
        {
            if (bossTrigger.backgroundMusic != null)
                bossTrigger.backgroundMusic.Stop();
            if (bossTrigger.bossMusic != null)
                bossTrigger.bossMusic.Stop();
        }
    }

    void StarKill(EnemyMovement enemy)
    {
        if (enemy == null)
            return;
        enemy.Squash();
        onIncrementScore.Raise(gameConstants.starKillScore);
        if (marioAudio != null && starKillSound != null)
            marioAudio.PlayOneShot(starKillSound);
    }

    void SetBigCollider(bool big)
    {
        marioCollider.size = big ? bigColliderSize : smallColliderSize;
        marioCollider.offset = big ? bigColliderOffset : smallColliderOffset;
    }

    void StopSizeAnimation()
    {
        if (sizeAnimation != null)
            StopCoroutine(sizeAnimation);
        sizeAnimation = null;
        spriteRenderer.enabled = true;
    }

    IEnumerator GrowAnimation()
    {
        for (int i = 0; i < sizeFlickers; i++)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(gameConstants.flickerInterval);
        }
        spriteRenderer.enabled = true;
        sizeAnimation = null;
    }

    private void RaiseGameOver()
    {
        onGameOver.Raise();
    }
}
