using UnityEngine;
using UnityEngine.Events;

public class SimpleGameEventListener : GameEventListener<Void>
{
    public UnityEvent simpleResponse;

    public override void OnEventRaised(Void data)
    {
        simpleResponse?.Invoke();
    }
}