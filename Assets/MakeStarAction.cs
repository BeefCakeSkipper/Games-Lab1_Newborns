using UnityEngine;

[CreateAssetMenu(menuName = "PluggableSM/Actions/MakeStar")]
public class MakeStarAction : Action
{
    public override void Act(StateController controller)
    {
        MarioStateController m = (MarioStateController)controller;
        m.MakeStar();
    }
}
