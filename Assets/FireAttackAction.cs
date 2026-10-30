using UnityEngine;

[CreateAssetMenu(menuName = "PluggableSM/Actions/FireAttack")]
public class FireAttackAction : Action
{
    public GameObject fireballPrefab;
    public AudioClip fireSound;
    public Vector2 spawnOffset = new Vector2(0.6f, 0.3f);

    public override void Act(StateController controller)
    {
        MarioStateController m = (MarioStateController)controller;
        if (fireballPrefab == null)
            return;

        int direction = m.GetComponent<SpriteRenderer>().flipX ? -1 : 1;
        Vector3 position = m.transform.position + new Vector3(spawnOffset.x * direction, spawnOffset.y, 0);
        GameObject fireball = Instantiate(fireballPrefab, position, Quaternion.identity);

        Physics2D.IgnoreCollision(fireball.GetComponent<Collider2D>(), m.GetComponent<Collider2D>());
        fireball.GetComponent<Fireball>().Launch(direction);
        m.GetComponent<Animator>().SetTrigger("onFire");

        if (m.marioAudio != null && fireSound != null)
            m.marioAudio.PlayOneShot(fireSound);
    }
}
