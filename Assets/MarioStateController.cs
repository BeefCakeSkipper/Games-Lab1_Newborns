using UnityEngine;

public class MarioStateController : StateController
{
    public PowerupType currentPowerupType = PowerupType.Default;
    public MarioState shouldBeNextState = MarioState.Default;

    public override void Start()
    {
        base.Start();
        GameRestart();
    }

    public void GameRestart()
    {
        currentPowerupType = PowerupType.Default;
        TransitionToState(startState);
    }

    public void SetPowerup(PowerupType i)
    {
        currentPowerupType = i;
    }
}
