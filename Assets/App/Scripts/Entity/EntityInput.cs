using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EntityInput : MonoBehaviour
{
    public Action<float> moveInput;
    float moveInputValue = 0;

    public Action compressDownInput;
    public Action compressUpInput;

    private void Update()
    {
        moveInput?.Invoke(moveInputValue);
    }

    public void MoveInput(InputAction.CallbackContext context)
    {
        moveInputValue = context.ReadValue<float>();
    }
    public void CompressInput(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                compressDownInput.Invoke();
                break;

            case InputActionPhase.Canceled:
                compressUpInput.Invoke();
                break;
        }
    }
}