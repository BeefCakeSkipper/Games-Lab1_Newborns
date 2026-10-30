using UnityEngine;

[CreateAssetMenu(menuName = "PluggableSM/Actions/EndStar")]
public class EndStarAction : Action
{
    public override void Act(StateController controller)
    {
        BuffStateController b = (BuffStateController)controller;
        b.mario.EndStar(true);
    }
}
