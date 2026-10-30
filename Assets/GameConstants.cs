using UnityEngine;

// shared game values, create one with Assets > Create > Scriptable Objects > GameConstants
[CreateAssetMenu(fileName = "GameConstants", menuName = "Scriptable Objects/GameConstants", order = 1)]
public class GameConstants : ScriptableObject
{
    [Header("Score")]
    public int startingScore = 0;     // score after a restart
    public int goombaStompScore = 1;
    public int starKillScore = 1;
    public int fireballKillScore = 1;
    public int bossHitScore = 1;

    [Header("Mario")]
    public Vector3 marioStartingPosition = new Vector3(-4.56f, -2.64f, 0.0f); // where mario starts after a scene change
    public float flickerInterval = 0.08f;

    [Header("Goomba")]
    public float goombaMaxOffset = 5.0f;   // how far a goomba walks from its start before turning
    public float goombaPatrolTime = 2.0f;

    [Header("Boss")]
    public float bossMaxOffset = 8.0f;
    public float bossPatrolTime = 2.0f;
}
