using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        Gameplay,
        Pause,
    }

    [Header("Settings")]
    [SerializeField] private GameState gameState;

    [Header("Input")]
    [SerializeField] private RSE_TogglePause rseTogglePause;
    [SerializeField] private RSE_OnGameStart rseOnGameStart;
    [SerializeField] private RSE_ClearBlobInGame rseClearBlobInGame;

    [Header("Output")]
    [SerializeField] private RSE_OnResume rseOnResume;
    [SerializeField] private RSE_OnPause rseOnPause;
    [SerializeField] private RSE_EnableWindow rseEnableWindow;
    [SerializeField] private RSE_DisableWindow rseDisableWindow;
    [SerializeField] private RSO_BlobInGame rsoBlobInGame;

    private void OnEnable()
    {
        rseTogglePause.action += TogglePause;
        rseOnGameStart.action += SetGameStateToGameplay;
        rseClearBlobInGame.action += ClearBlobInGame;
    }

    private void OnDisable()
    {
        rseTogglePause.action -= TogglePause;
        rseOnGameStart.action -= SetGameStateToGameplay;
        rseClearBlobInGame.action -= ClearBlobInGame;
    }

    private void Awake()
    {
        rsoBlobInGame.Value = new();
    }

    private void TogglePause()
    {
        switch (gameState)
        {
            case GameState.Gameplay:
                gameState = GameState.Pause;
                rseEnableWindow.Call("PausePanel");
                rseOnPause.Call();
                break;

            case GameState.Pause:
                gameState = GameState.Gameplay;
                rseDisableWindow.Call("Settings");
                rseDisableWindow.Call("PausePanel");
                rseOnResume.Call();
                break;
        }
    }

    private void SetGameStateToGameplay()
    {
        gameState = GameState.Gameplay;
    }

    private void ClearBlobInGame()
    {
        if (rsoBlobInGame.Value.Count > 0)
        {
            for (int i = 0; i < rsoBlobInGame.Value.Count; i++)
            {
                Destroy(rsoBlobInGame.Value[i].gameObject);
            }

            rsoBlobInGame.Value.Clear();
        }
    }
}