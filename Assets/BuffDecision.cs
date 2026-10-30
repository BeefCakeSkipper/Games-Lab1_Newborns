using UnityEngine;

[CreateAssetMenu(menuName = "PluggableSM/Decisions/Buff")]
public class BuffDecision : Decision
{
    public BuffTransformMap[] map;

    public override bool Decide(StateController controller)
    {
        BuffStateController b = (BuffStateController)controller;
        // same idea as TransformDecision: the state asset is named after a BuffState value
        BuffState toCompareState = EnumExtension.ParseEnum<BuffState>(b.currentState.name);

        for (int i = 0; i < map.Length; i++)
        {
            if (toCompareState == map[i].fromState && b.currentBuffType == map[i].powerupCollected)
            {
                return true;
            }
        }

        return false;
    }
}

[System.Serializable]
public struct BuffTransformMap
{
    public BuffState fromState;
    public PowerupType powerupCollected;
}
