using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ButtonNavigationGamepad : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button defaultButton;

    private void OnEnable()
    {
        if (Gamepad.current != null)
        {
            defaultButton.Select();
            defaultButton.GetComponent<InteractiveButton>()?.OnSelected();
        }
    }
}