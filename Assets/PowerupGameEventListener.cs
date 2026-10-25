using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class PowerupUnityEvent : UnityEvent<IPowerup>
{
}

public class PowerupGameEventListener : GameEventListener<IPowerup>
{
    public PowerupUnityEvent powerupResponse;

    public override void OnEventRaised(IPowerup data)
    {
        powerupResponse?.Invoke(data);
    }
}