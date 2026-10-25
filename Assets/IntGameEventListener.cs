using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class IntUnityEvent : UnityEvent<int>
{
}

public class IntGameEventListener : GameEventListener<int>
{
    public IntUnityEvent intResponse;

    public override void OnEventRaised(int data)
    {
        intResponse?.Invoke(data);
    }
}