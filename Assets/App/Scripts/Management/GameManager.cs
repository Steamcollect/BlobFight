using UnityEngine;
public class GameManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] GameState gameState;
    public enum GameState
    {
        Gameplay,
        Pause
    }

    //[Header("References")]

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    [Header("Input")]
    [SerializeField] RSE_TogglePause rseTogglePause;

    [Header("Output")]
    [SerializeField] RSE_OnResume rseOnResume;
    [SerializeField] RSE_OnPause rseOnPause;

    private void OnEnable()
    {
        rseTogglePause.action += TogglePause;
    }
    private void OnDisable()
    {
        rseTogglePause.action -= TogglePause;
    }

    void TogglePause()
    {
        switch (gameState)
        {
            case GameState.Gameplay:
                gameState = GameState.Pause;
                rseOnResume.Call();
                break;

            case GameState.Pause:
                gameState = GameState.Gameplay;
                rseOnPause.Call();
                break;
        }
    }
}