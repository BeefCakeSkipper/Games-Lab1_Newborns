using UnityEngine;

[CreateAssetMenu(menuName = "PluggableSM/Actions/EndInvincible")]
public class EndInvincibleAction : Action
{
    public override void Act(StateController controller)
    {
        controller.GetComponent<PlayerMovement>().EndInvincible();
    }
}
