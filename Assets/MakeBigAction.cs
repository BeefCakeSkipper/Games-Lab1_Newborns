using UnityEngine;

[CreateAssetMenu(menuName = "PluggableSM/Actions/MakeBig")]
public class MakeBigAction : Action
{
    public override void Act(StateController controller)
    {
        controller.GetComponent<PlayerMovement>().MakeBig();
    }
}
