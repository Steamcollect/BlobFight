using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameManager;

public class SceneManagement : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool isTestScene;
    [SerializeField] private SceneReference[] levelsName;
    [SerializeField] private SceneReference mainMain;
    [SerializeField] private SceneReference mainMenuName;

    [Header("Input")]
    [SerializeField] private RSE_LoadNextLevel rseLoadNextLevel;
    [SerializeField] private RSE_ReturnToMainMenu rseReturnToMainMenu;
    [SerializeField] private RSE_Quit rseQuit;

    [Header("Output")]
    [SerializeField] private RSE_FadeOut rseFadeOut;
    [SerializeField] private RSE_EnableJoining rseEnableJoining;
    [SerializeField] private RSE_ClearBlobInGame rseClearBlobInGame;
    [SerializeField] private RSE_TogglePause rseTogglePause;

    private List<string> levels = new();
    private string currentLevel = "";
    private bool isLoading = false;

    private void OnEnable()
    {
        rseLoadNextLevel.action += LoadNextLevelRandomly;
        rseReturnToMainMenu.action += ReturnToMainMenu;
        rseQuit.action += QuitGame;
    }

    private void OnDisable()
    {
        rseLoadNextLevel.action -= LoadNextLevelRandomly;
        rseReturnToMainMenu.action -= ReturnToMainMenu;
        rseQuit.action -= QuitGame;
    }

    private void Start()
    {
        if (!isTestScene)
        {
            StartCoroutine(Utils.LoadSceneAsync(mainMenuName.Name, LoadSceneMode.Additive));
            currentLevel = mainMenuName.Name;
        }
        else
        {
            currentLevel = SceneManager.GetActiveScene().name;
            isLoading = false;
        }
    }

    private void LoadNextLevelRandomly()
    {
        LoadLevel(false);
    }

    private void ReturnToMainMenu()
    {
		rseFadeOut.Call(() =>
		{
			LoadLevel(true);
		});
    }

    private void QuitGame()
    {
        rseFadeOut.Call(() =>
        {
            Application.Quit();
        });
    }

    private void LoadLevel(bool isMainMenu)
    {
        if (isLoading) return;

        isLoading = true;

        if (!isTestScene)
        {
            Transition(isMainMenu);
        }
        else
        {
            InstanteTransition(isMainMenu);
        }
    }

    private string GetRandomLevel()
    {
        if (levels.Count == 0)
        {
            levels.AddRange(levelsName.Select(level => level.Name));
        }

        int index = Random.Range(0, levels.Count);
        string selectedLevel = levels[index];
        levels.RemoveAt(index);

        return selectedLevel;
    }

    private void Transition(bool isMainMenu)
    {
        string previousLevel = currentLevel;

        if (!isMainMenu)
		{
            currentLevel = GetRandomLevel();
		}
		else
		{
			rseClearBlobInGame.Call();
			currentLevel = mainMenuName.Name;
            rseTogglePause.Call();
        }

        if (!string.IsNullOrEmpty(previousLevel))
        {
            StartCoroutine(Utils.UnloadSceneAsync(previousLevel));
        }

        StartCoroutine(Utils.LoadSceneAsync(currentLevel, LoadSceneMode.Additive, () =>
        {
			isLoading = false;

            if (isMainMenu)
            {
                rseEnableJoining.Call();
            }
        }));
    }

    private void InstanteTransition(bool isMainMenu)
    {
        if (isMainMenu)
        {
            SceneManager.LoadScene(mainMain.Name);
        }
        else
        {
            SceneManager.LoadScene(currentLevel);
        }
    }
}