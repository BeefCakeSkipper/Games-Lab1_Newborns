using UnityEngine;

// runs alongside MarioStateController: that one handles mario's size, this one handles buffs like starman
public class BuffStateController : StateController
{
    public PowerupType currentBuffType = PowerupType.Default;

    // the star effect itself (colours, music, star kills) still lives on MarioStateController
    [HideInInspector] public MarioStateController mario;

    private void Awake()
    {
        mario = GetComponent<MarioStateController>();
    }

    public override void Start()
    {
        base.Start();
        GameRestart();
    }

    public void GameRestart()
    {
        currentBuffType = PowerupType.Default;
        TransitionToState(startState);
    }

    public void SetBuff(PowerupType i)
    {
        currentBuffType = i;
    }
}
