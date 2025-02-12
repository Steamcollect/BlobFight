using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EntityInput : MonoBehaviour
{
    public Action<Vector2> moveInput;
    Vector2 moveInputValue;

    public Action dashInput;

    public Action compressDownInput;
    public Action compressUpInput;

    private void Update()
    {
        moveInput?.Invoke(moveInputValue);
    }

    public void MoveInput(InputAction.CallbackContext context)
    {
        moveInputValue = context.ReadValue<Vector2>();
    }
    public void DashInput(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                dashInput?.Invoke();
                break;
        }
    }
    public void CompressInput(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                compressDownInput?.Invoke();
                break;

            case InputActionPhase.Canceled:
                compressUpInput?.Invoke();
                break;
        }
    }
}