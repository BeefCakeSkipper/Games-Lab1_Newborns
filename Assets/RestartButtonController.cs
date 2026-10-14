using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartButtonController : MonoBehaviour, IInteractiveButton
{
    public string firstLevel = "World 1-1"; // restart always goes back to the first level

    // implements the interface
    public void ButtonClick()
    {
        Debug.Log("Onclick restart button");

        // the end screen paused the game
        Time.timeScale = 1f;
        AudioListener.pause = false;

        // mario, the HUD and the GameManager survive scene changes, throw them away so
        // the first level loads its own fresh ones (score back to 0, every level reloaded)
        if (PlayerMovement.instance != null)
            Destroy(PlayerMovement.instance.gameObject);
        if (HUDManager.instance != null)
            Destroy(HUDManager.instance.gameObject);
        if (GameManager.instance != null)
            Destroy(GameManager.instance.gameObject);

        SceneManager.LoadScene(firstLevel);
    }
}
