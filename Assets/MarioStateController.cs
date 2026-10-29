using System;
using System.Collections;
using UnityEngine;

public class MarioStateController : StateController
{
    public GameConstants gameConstants;
    public PowerupType currentPowerupType = PowerupType.Default;
    public MarioState shouldBeNextState = MarioState.Default;
    private SpriteRenderer spriteRenderer;

    public override void Start()
    {
        base.Start();
        GameRestart();
    }

    public void GameRestart()
    {
        currentPowerupType = PowerupType.Default;
        TransitionToState(startState);
    }

    public void SetPowerup(PowerupType i)
    {
        currentPowerupType = i;
    }
    public void SetRendererToFlicker()
    {
        StartCoroutine(BlinkSpriteRenderer());
    }
    private IEnumerator BlinkSpriteRenderer()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        while (string.Equals(currentState.name, "InvincibleSmallMario", StringComparison.OrdinalIgnoreCase))
        {
            // Toggle the visibility of the sprite renderer
            spriteRenderer.enabled = !spriteRenderer.enabled;

            // Wait for the specified blink interval
            yield return new WaitForSeconds(gameConstants.flickerInterval);
        }

        spriteRenderer.enabled = true;
    }

}
