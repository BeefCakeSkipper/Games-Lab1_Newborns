using UnityEngine;

[CreateAssetMenu(menuName = "PluggableSM/Actions/KillMario")]
public class KillMarioAction : Action
{
    public override void Act(StateController controller)
    {
        controller.GetComponent<PlayerMovement>().KillMario();
    }
}
