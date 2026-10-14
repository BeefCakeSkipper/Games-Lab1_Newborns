using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadingManager : MonoBehaviour
{
    public Slider progressBar;
    public GameObject playButton;
    public GameObject backButton;

    public float loadingDuration = 3f;

    private bool sceneReady = false;

    void Start()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;

        progressBar.value = 0f;

        playButton.SetActive(false);
        backButton.SetActive(false);

        StartCoroutine(AnimateLoading());
    }

    IEnumerator AnimateLoading()
    {
        float elapsed = 0f;

        while (elapsed < loadingDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            progressBar.value =
                Mathf.Clamp01(elapsed / loadingDuration);

            yield return null;
        }

        progressBar.value = 1f;
        sceneReady = true;

        playButton.SetActive(true);
        backButton.SetActive(true);

        Debug.Log("Loading complete. Buttons enabled.");
    }

    public void PlayGame()
    {
        Debug.Log("Play button clicked!");

        if (!sceneReady)
            return;

        int nextSceneIndex =
            SceneManager.GetActiveScene().buildIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.LogError("No next scene available!");
        }
    }

    public void BackToMainMenu()
    {
        Debug.Log("Back button clicked!");

        SceneManager.LoadScene("MainMenu");
    }
}