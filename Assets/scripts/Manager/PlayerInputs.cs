using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public static class PlayerInputs
{
    static readonly int buffer = 100;
    static readonly ActionMap inputs;
    static readonly InputAction runAction, jumpAction, interactAction;
    public static Vector2 MoveInput
    {
        get => inputs.PlayerControls.Move.ReadValue<Vector2>();
    }
    public static Vector2 LookDelta
    {
        get => inputs.PlayerControls.Look.ReadValue<Vector2>();
    }

    public static bool RunInput { get; private set; } = false;
    public static bool JumpPress { get; private set; } = false;
    public static bool JumpHold { get; private set; } = false;
    public static bool InteractPress { get; private set; } = false;
    public static bool InteractHold { get; private set; } = false;
    static PlayerInputs()
    {
        inputs = new ActionMap();
        runAction = inputs.FindAction("Run");
        jumpAction = inputs.FindAction("Jump");
        interactAction = inputs.FindAction("Interact");
        OnEnable();
    }

    private static void OnEnable()
    {
        inputs.Enable();
        runAction.started += (_) => RunInput = true;
        runAction.canceled += (_) => RunInput = false;
        jumpAction.started += (_) => { JumpHold = true; BufferJump(); };
        jumpAction.canceled += (_) => JumpHold = false;
        interactAction.started += (_) => { InteractHold = true; BufferInteract(); };
        interactAction.canceled += (_) => InteractHold = false;
    }

    private static void BufferJump()
    {
        JumpPress = true;
        Wait(buffer).ContinueWith(_ => JumpPress = false);
    }
    private static void BufferInteract()
    {
        InteractPress = true;
        Wait(buffer).ContinueWith(_ => InteractPress = false);
    }

    public static async Task Wait(int milliseconds)
    {
        await Task.Run(() =>
        {
            Task.Delay(milliseconds).Wait();
        });
    }
}
