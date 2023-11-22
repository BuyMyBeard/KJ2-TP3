using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Windows;
public class PlayerController : MonoBehaviour
{
    public Vector2 Look { get; private set; } = Vector2.zero;
    public Vector2 Move { get; private set; } = Vector2.zero;
    public bool IsSprinting { get; private set; } = false;

    public string CurrentControlScheme { get => playerInput.currentControlScheme; }
    public bool KeyboardAndMouseActive { get => CurrentControlScheme == "Keyboard&Mouse"; }
    public bool GamepadActive { get => CurrentControlScheme == "Gamepad"; }

    InputAction runAction;

    PlayerInput playerInput;
    public void SwitchToPlayerControls() => playerInput.SwitchCurrentActionMap("PlayerControls");
    public void SwitchToUI() => playerInput.SwitchCurrentActionMap("UI");
    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        runAction = playerInput.currentActionMap.FindAction("RunButton");
    }
    private void OnEnable()
    {
        runAction.performed += Run_performed;
        runAction.canceled += Run_canceled;
    }
    private void OnDisable()
    {
        runAction.performed -= Run_performed;
        runAction.canceled -= Run_canceled;
    }

    private void Run_canceled(InputAction.CallbackContext obj)
    {
        IsSprinting = false;
    }
    // Necessary because this is a hold interaction
    private void Run_performed(InputAction.CallbackContext obj)
    {
        IsSprinting = true;
    }
    void OnDodge()
    {
        Debug.Log("dodged");
    }
    void OnRun()
    {
        Debug.Log("ran");
    }

    void OnMove(InputValue val) => Move = val.Get<Vector2>();
    void OnLook(InputValue val) => Look = val.Get<Vector2>();
}

