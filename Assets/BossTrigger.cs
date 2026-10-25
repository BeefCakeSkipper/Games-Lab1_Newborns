// Hello World     
using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    [Header("Boss UI")]
    [SerializeField] private GameObject bossHealthUI;
    [SerializeField] private PlayerMovement player;
    public CameraController CameraController;
    public GameObject boss;
    public bool bossTriggered = false;

    [Header("Boss Arena Walls")]
    [SerializeField] private GameObject leftWall;
    [SerializeField] private GameObject rightWall;

    public GameObject bossEnvironment;
    public AudioSource backgroundMusic;
    public AudioSource bossMusic;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        boss.SetActive(false);

        if (leftWall != null)
            leftWall.SetActive(false);

        if (rightWall != null)
            rightWall.SetActive(false);

        if (bossHealthUI != null)
            bossHealthUI.SetActive(false);

        bossEnvironment.SetActive(false);
    }

    public void GameRestart()
    {
        BossLogic bossLogic = boss.GetComponent<BossLogic>();
        if (bossLogic != null)
            bossLogic.Respawn();
        ResetTrigger();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        PlayerMovement mario = col.GetComponent<PlayerMovement>();

        if (!bossTriggered && mario != null)
        {
            Debug.Log("[BossTrigger] Mario entered boss arena.");

            bossTriggered = true;

            if (CameraController != null)
                CameraController.enabled = false;

            // Activate boss.
            boss.SetActive(true);

            // Activate arena walls.
            if (leftWall != null)
                leftWall.SetActive(true);

            if (rightWall != null)
                rightWall.SetActive(true);

            // Activate boss health UI.
            if (bossHealthUI != null)
            {
                Debug.Log(
                    $"[BossTrigger] BEFORE activation: " +
                    $"activeSelf={bossHealthUI.activeSelf}, " +
                    $"activeInHierarchy={bossHealthUI.activeInHierarchy}"
                );

                bossHealthUI.SetActive(true);

                Debug.Log(
                    $"[BossTrigger] AFTER activation: " +
                    $"activeSelf={bossHealthUI.activeSelf}, " +
                    $"activeInHierarchy={bossHealthUI.activeInHierarchy}"
                );

                Canvas canvas = bossHealthUI.GetComponentInParent<Canvas>();

                if (canvas != null)
                {
                    Debug.Log(
                        $"[BossTrigger] Canvas={canvas.name}, " +
                        $"renderMode={canvas.renderMode}, " +
                        $"enabled={canvas.enabled}, " +
                        $"activeInHierarchy={canvas.gameObject.activeInHierarchy}"
                    );
                }
                else
                {
                    Debug.LogWarning("[BossTrigger] No parent Canvas found!");
                }
            }
            else
            {
                Debug.LogError("[BossTrigger] BossHealthUI is not assigned!");
            }

            bossEnvironment.SetActive(true);

            backgroundMusic.Stop();
            bossMusic.Play();

            mario.MusicChanged(bossMusic);
        }
    }
    public void ResetTrigger()
    {
        bossTriggered = false;

        boss.SetActive(false);

        if (leftWall != null)
            leftWall.SetActive(false);

        if (rightWall != null)
            rightWall.SetActive(false);
        Debug.LogWarning("[BossLogic.Respawn] Disabling BossHealthUI");
        if (bossHealthUI != null)
            bossHealthUI.SetActive(false);

        bossEnvironment.SetActive(false);

        if (CameraController != null)
            CameraController.enabled = true;

        bossMusic.Stop();
        backgroundMusic.Play();
    }
    public void HideBossHealthUI()
    {
        Debug.LogWarning("[BossLogic.Respawn] Disabling BossHealthUI");
        if (bossHealthUI != null)
            bossHealthUI.SetActive(false);
    }
    private void LateUpdate()
    {
        BossLogic bossLogic = boss != null
            ? boss.GetComponent<BossLogic>()
            : null;

        if (bossTriggered &&
            player != null &&
            player.alive &&
            bossLogic != null &&
            !bossLogic.IsDead &&
            bossHealthUI != null &&
            !bossHealthUI.activeSelf)
        {
            Debug.LogError(
                "[BossTrigger] BossHealthUI unexpectedly disabled during boss fight!",
                bossHealthUI
            );
        }
    }
}
