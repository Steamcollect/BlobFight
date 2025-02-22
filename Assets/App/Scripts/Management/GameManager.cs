using UnityEngine;
public class GameManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] GameState gameState = GameState.MainMenu;
    public enum GameState
    {
        Gameplay,
        Pause,
        MainMenu
    }

    //[Header("References")]

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    [Header("Input")]
    [SerializeField] RSE_TogglePause rseTogglePause;
    [SerializeField] RSE_OnGameStart rseOnGameStart;
    [SerializeField] RSE_ReturnToMainMenu rseReturnToMainMenu;

    [Header("Output")]
    [SerializeField] RSE_OnResume rseOnResume;
    [SerializeField] RSE_OnPause rseOnPause;

    [Space(10)]
    [SerializeField] RSE_EnableWindow rseEnableWindow;
    [SerializeField] RSE_DisableWindow rseDisableWindow;

    private void OnEnable()
    {
        rseTogglePause.action += TogglePause;
        rseOnGameStart.action += SetStateToGameplay;
        rseReturnToMainMenu.action += SetStateToMainMenu;
    }
    private void OnDisable()
    {
        rseTogglePause.action -= TogglePause;
        rseOnGameStart.action -= SetStateToGameplay;
        rseReturnToMainMenu.action -= SetStateToMainMenu;
    }

    void TogglePause()
    {
        if (gameState == GameState.MainMenu) return;

        switch (gameState)
        {
            case GameState.Gameplay:
                gameState = GameState.Pause;
                rseEnableWindow.Call("PausePanel");
                rseOnPause.Call();
                break;

            case GameState.Pause:
                gameState = GameState.Gameplay;
                rseDisableWindow.Call("PausePanel");
                rseOnResume.Call();
                break;
        }
    }

    void SetStateToMainMenu()
    {
        gameState = GameState.MainMenu;
    }
    void SetStateToGameplay()
    {
        gameState = GameState.Gameplay;
    }
}