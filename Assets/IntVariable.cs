
using UnityEngine;

[CreateAssetMenu(fileName = "NewIntVariable", menuName = "ScriptableObjects/Int Variable")]
public class IntVariable : ScriptableObject
{
    [SerializeField] private IntVariable gameScore;
    [SerializeField] private int initialValue = 0;

    [System.NonSerialized]
    private int runtimeValue;

    [System.NonSerialized]
    private bool initialized;



    public int Value
    {
        get
        {
            if (!initialized)
            {
                ResetValue();
            }

            return runtimeValue;
        }
        set
        {
            runtimeValue = value;
            initialized = true;
        }
    }

    public void ApplyChange(int amount)
    {
        Value += amount;
    }

    public void ResetValue()
    {
        runtimeValue = initialValue;
        initialized = true;
    }
}
