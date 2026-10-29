using UnityEngine;

[CreateAssetMenu(menuName = "PluggableSM/Actions/MakeBig")]
public class MakeBigAction : Action
{
    public AudioClip powerupSound;
    public override void Act(StateController controller)
    {
        MarioStateController m = (MarioStateController)controller;
        if (m.isBig)
            return;
        if (m.marioAudio != null && powerupSound != null)
            m.marioAudio.PlayOneShot(powerupSound);
        m.MakeBig();
    }
}
