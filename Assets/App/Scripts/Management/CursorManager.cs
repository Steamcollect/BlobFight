using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class S_CursorManager : MonoBehaviour
{
    private void OnEnable()
    {
        InputSystem.onDeviceChange += OnDeviceChange;
    }

    private void OnDisable() 
    {
        InputSystem.onDeviceChange -= OnDeviceChange;
    }

    private void Start()
    {
        if (Gamepad.current != null)
        {
            HideMouseCursor();
        }
        else 
        {
            ShowMouseCursor();
        }
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (device is Gamepad)
        {
            if (change == InputDeviceChange.Added)
            {
                HideMouseCursor();
            }
            else if (change == InputDeviceChange.Removed && Gamepad.all.Count == 0)
            {
                EventSystem.current.SetSelectedGameObject(null);

                ShowMouseCursor();
            }
        }
    }

    private void ShowMouseCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    private void HideMouseCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}
