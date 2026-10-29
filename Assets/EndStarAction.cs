using UnityEngine;

[CreateAssetMenu(menuName = "PluggableSM/Actions/EndStar")]
public class EndStarAction : Action
{
    public override void Act(StateController controller)
    {
        MarioStateController m = (MarioStateController)controller;
        m.EndStar(true);
    }
}
