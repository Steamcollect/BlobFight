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

    public Action pauseInput;

    public Action<bool> validateInput;
    public Action returnInput;

    public Action breakVialInput;

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

    public void PauseInput(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started)
        {
            pauseInput?.Invoke();
        }
    }

    public void ValidateInput(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                validateInput?.Invoke(true);
                break;

            case InputActionPhase.Canceled:
                validateInput?.Invoke(false);
                break;
        }
    }
    public void ReturnInput(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                returnInput?.Invoke();
                break;
        }
    }

    public void BreakVialInput(InputAction.CallbackContext context)
    {
        switch(context.phase)
        {
            case InputActionPhase.Started:
                breakVialInput?.Invoke();
                break;
        }
    }

    public void OnControllerDisconnect()
    {
        Destroy(gameObject);
    }
}