using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        Debug.Log("Start button clicked!");
        SceneManager.LoadScene("LoadingScene");
    }
}