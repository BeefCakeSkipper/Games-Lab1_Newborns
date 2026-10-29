using UnityEngine;

[CreateAssetMenu(menuName = "PluggableSM/Actions/KillMario")]
public class KillMarioAction : Action
{
    public AudioClip deathSound;
    public override void Act(StateController controller)
    {
        MarioStateController m = (MarioStateController)controller;
        if (m.marioDeathAudio != null && deathSound != null)
            m.marioDeathAudio.PlayOneShot(deathSound);
        m.KillMario();
    }
}
