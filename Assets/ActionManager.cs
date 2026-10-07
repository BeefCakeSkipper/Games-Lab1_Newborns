using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ActionManager : MonoBehaviour
{
    public MarioActions marioActions;
    public PlayerInput playerInput;
    private InputAction jumpHoldAction;

    void Start()
    {
        jumpHoldAction = playerInput.actions["jumphold"];
        jumpHoldAction.performed += OnJumpHoldPerformed;
        marioActions = new MarioActions();
        marioActions.gameplay.Enable();
        marioActions.gameplay.jump.performed += OnJump;
        marioActions.gameplay.jumphold.performed += OnJumpHoldPerformed;
        marioActions.gameplay.move.started += OnMoveAction;
        marioActions.gameplay.move.canceled += OnMoveAction;
    }

    private void OnJumpHoldPerformed(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }


    // triggered upon performed interaction (default successful press)
    public void OnJump(InputAction.CallbackContext contex)
    {
        Debug.Log("OnJump called");
        // TODO
    }

    // triggered upon 1D value change (default successful press and cancelled)
    void OnMove(InputValue input)
    {
        if (input.Get() == null)
        {
            Debug.Log("Move released");
        }
        else
        {
            Debug.Log($"Move triggered, with value {input.Get()}"); // will return null when released
        }
        // TODO
    }

    // triggered upon performed interaction (custom successful hold)
    public void OnJumphold(InputValue value)
    {
        Debug.Log($"OnJumpHold performed with value {value.Get()}");
        // TODO

    }
    public void OnJumpHoldAction(InputAction.CallbackContext context)
    {
        if (context.started)
            Debug.Log("JumpHold was started");
        else if (context.performed)
        {
            Debug.Log("JumpHold was performed");
        }
        else if (context.canceled)
            Debug.Log("JumpHold was cancelled");
    }

    // called twice, when pressed and unpressed
    public void OnJumpAction(InputAction.CallbackContext context)
    {
        if (context.started)
            Debug.Log("Jump was started");
        else if (context.performed)
        {
            Debug.Log("Jump was performed");
        }
        else if (context.canceled)
            Debug.Log("Jump was cancelled");

    }

    // called twice, when pressed and unpressed
    public void OnMoveAction(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Debug.Log("move started");
            float move = context.ReadValue<float>();
            Debug.Log($"move value: {move}"); // will return null when not pressed
        }
        if (context.canceled)
        {
            Debug.Log("move stopped");
        }
    }

}
