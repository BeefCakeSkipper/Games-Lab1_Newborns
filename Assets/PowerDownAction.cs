using UnityEngine;

[CreateAssetMenu(menuName = "PluggableSM/Actions/PowerDown")]
public class PowerDownAction : Action
{
    public override void Act(StateController controller)
    {
        MarioStateController m = (MarioStateController)controller;
        m.PowerDown();
    }
}
