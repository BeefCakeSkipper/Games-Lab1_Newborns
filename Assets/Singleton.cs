using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    public static T instance
    {
        get
        {
            return _instance;
        }
    }

    public virtual void Awake()
    {
        Debug.Log($"Awake called: {gameObject.name}, ID: {GetInstanceID()}");

        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);

            Debug.Log($"Mario registered: {gameObject.name}");
        }
        else if (_instance != this)
        {
            Debug.LogWarning($"Duplicate Mario destroyed: {gameObject.name}");
            Destroy(gameObject);
        }
    }
}