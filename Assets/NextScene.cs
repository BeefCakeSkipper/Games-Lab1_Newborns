
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class NextScene : MonoBehaviour
{
    public string nextSceneName;
    private bool loading = false;
    void OnTriggerEnter2D(Collider2D other)
    {
        // only load once, even if mario touches the trigger again while it's loading
        if (other.tag == "Player" && !loading)
        {
            loading = true;
            Debug.Log("Change scene!");
            SceneManager.LoadSceneAsync(nextSceneName, LoadSceneMode.Single);
        }
    }
}