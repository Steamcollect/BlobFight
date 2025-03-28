using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManagement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInputManager playerInputManager;

    [Header("Input")]
    [SerializeField] private RSE_EnableJoining rseEnableJoining;
    [SerializeField] private RSE_DisableJoining rseDisableJoining;

	private void OnEnable()
    {
		playerInputManager.onPlayerJoined += OnPlayerJoined;
		rseEnableJoining.action += playerInputManager.EnableJoining;
        rseDisableJoining.action += playerInputManager.DisableJoining;
    }

    private void OnDisable()
    {
		playerInputManager.onPlayerJoined -= OnPlayerJoined;
		rseEnableJoining.action -= playerInputManager.EnableJoining;
        rseDisableJoining.action -= playerInputManager.DisableJoining;
    }

    private void OnPlayerJoined(PlayerInput playerInput)
	{
        playerInput.GetComponent<BlobMotor>()?.OnJoined();
    }
}