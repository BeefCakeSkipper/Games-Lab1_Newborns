using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    public CameraFollow cameraFollow;
    public GameObject boss;
    public bool bossTriggered = false;
    // public GameObject leftWall;
    // public GameObject rightWall;
    public GameObject bossHealth;
    public GameObject bossEnvironment;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("BossTrigger Start running");
        boss.SetActive(false);
        // leftWall.SetActive(false);
        // rightWall.SetActive(false);
        bossHealth.SetActive(false);
        bossEnvironment.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        // only mario starts the fight
        if (!bossTriggered && col.GetComponent<PlayerMovement>() != null)
        {
            Debug.Log("BossTrigger triggered by: " + col.gameObject.name);
            bossTriggered = true;
            cameraFollow.enabled = false;
            boss.SetActive(true);
            // leftWall.SetActive(true);
            // rightWall.SetActive(true);
            bossHealth.SetActive(true);
            bossEnvironment.SetActive(true);
        }

    }

    public void ResetTrigger()
    {
        bossTriggered = false;
        boss.SetActive(false);
        // leftWall.SetActive(false);
        // rightWall.SetActive(false);
        bossHealth.SetActive(false);
        bossEnvironment.SetActive(false);
    }
}
