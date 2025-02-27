using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] bool canPause = true;
    [SerializeField] GameState gameState = GameState.MainMenu;
    public enum GameState
    {
        Gameplay,
        Pause,
        MainMenu
    }

    [Header("References")]

    //[Space(10)]
    // RSO
    [SerializeField] RSO_BlobInGame rsoBlobInGame;
    // RSF
    // RSP

    [Header("Input")]
    [SerializeField] RSE_TogglePause rseTogglePause;
    [SerializeField] RSE_OnGameStart rseOnGameStart;
    [SerializeField] RSE_ReturnToMainMenu rseReturnToMainMenu;

    [Space(5)]
    [SerializeField] RSE_EnablePauseAction rseEnablePauseAction;
    [SerializeField] RSE_DisablePauseAction rseDisablePauseAction;

    [Space(5)]
    [SerializeField] RSE_ClearBlobInGame rseClearBlobInGame;

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

        rseEnablePauseAction.action += EnablePause;
        rseDisablePauseAction.action += DisablePause;

        rseClearBlobInGame.action += ClearBlobInGame;
    }
    private void OnDisable()
    {
        rseTogglePause.action -= TogglePause;
        rseOnGameStart.action -= SetStateToGameplay;
        rseReturnToMainMenu.action -= SetStateToMainMenu;

        rseEnablePauseAction.action -= EnablePause;
        rseDisablePauseAction.action -= DisablePause;

        rseClearBlobInGame.action -= ClearBlobInGame;
    }

    private void Awake()
    {
        rsoBlobInGame.Value = new();
    }

    void TogglePause()
    {
        if (!canPause) return;

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

            case GameState.MainMenu:
                gameState = GameState.Pause;
                rseEnableWindow.Call("PausePanel");
                rseOnPause.Call();
                break;
        }
    }

    void SetStateToMainMenu()
    {
        rseDisableWindow.Call("PausePanel");
        gameState = GameState.MainMenu;
    }
    void SetStateToGameplay()
    {
        gameState = GameState.Gameplay;
    }

    void EnablePause()
    {
        canPause = true;
    }
    void DisablePause()
    {
        canPause = false;
    }

    void ClearBlobInGame()
    {
        if (rsoBlobInGame.Value.Count <= 0) return;

        for (int i = 0; i < rsoBlobInGame.Value.Count; i++)
        {
            Destroy(rsoBlobInGame.Value[i].gameObject);
        }
        rsoBlobInGame.Value.Clear();
    }
}