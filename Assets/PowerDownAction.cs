using UnityEngine;

[CreateAssetMenu(menuName = "PluggableSM/Actions/PowerDown")]
public class PowerDownAction : Action
{
    public override void Act(StateController controller)
    {
        controller.GetComponent<PlayerMovement>().PowerDown();
    }
}
