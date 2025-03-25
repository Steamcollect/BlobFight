using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{
    [Header("Settings")]

    [SerializeField] bool isTestScene = false;

    [Space(10)]

    [SerializeField] SceneReference[] levelsName;

    List<string> levels = new();
    [SerializeField] SceneReference main;
    [SerializeField] SceneReference mainMenuName;

    string currentLevel = "";

    bool isLoading = false;

    [Header("References")]

    //[Space(10)]
    // RSO
    [SerializeField] RSO_BlobInGame rsoBlobInGame;
    // RSF
    // RSP

    [Header("Input")]
    [SerializeField] RSE_LoadNextLevel rseLoadNextLevel;
    [SerializeField] RSE_ReturnToMainMenu rseReturnToMainMenu;
    [SerializeField] RSE_Quit rseQuit;

    [Header("Output")]
    [SerializeField] RSE_FadeIn rseFadeIn;
    [SerializeField] RSE_FadeOut rseFadeOut;

    [Space(5)]
    [SerializeField] RSE_EnablePauseAction rseEnablePauseAction;
    [SerializeField] RSE_DisablePauseAction rseDisablePauseAction;

    [Space(5)]
    [SerializeField] RSE_ClearBlobInGame rseClearBlobInGame;

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
        if(!isTestScene)
        {
            StartCoroutine(Utils.LoadSceneAsync(mainMenuName.Name, LoadSceneMode.Additive));
            currentLevel = mainMenuName.Name;
        }
        else
        {
            currentLevel = SceneManager.GetActiveScene().name;
            rseEnablePauseAction.Call();
            isLoading = false;
        }
    }

    void LoadNextLevelRandomly()
    {
        LoadLevel(false);
    }

    void ReturnToMainMenu()
    {
        LoadLevel(true);
    }

    void LoadLevel(bool isMainMenu)
    {
        if (isLoading) return;

        rseDisablePauseAction.Call();
        isLoading = true;

        if (!isTestScene) TransitionWithFade(isMainMenu);
        else InstanteTransition(isMainMenu);
    }

    void TransitionWithFade(bool isMainMenu)
    {
		if (currentLevel != "")
		{
			StartCoroutine(Utils.UnloadSceneAsync(currentLevel));
		}

		if (!isMainMenu)
		{
			if (levels.Count <= 0)
			{
				foreach (var item in levelsName)
				{
					levels.Add(item.Name);
				}
			}

			int rnd = Random.Range(0, levels.Count);

			currentLevel = levels[rnd];

			levels.RemoveAt(rnd);
		}
		else
		{
			rseClearBlobInGame.Call();
			currentLevel = mainMenuName.Name;
		}

		StartCoroutine(Utils.LoadSceneAsync(currentLevel, LoadSceneMode.Additive, () =>
        {
			rseEnablePauseAction.Call();
			isLoading = false;
		}));
	}
    void InstanteTransition(bool isMainMenu)
    {
        if(isMainMenu)
        {
            SceneManager.LoadScene(main.Name);
        }
        else
        {
            SceneManager.LoadScene(currentLevel);
        }
    }

    void QuitGame()
    {
        rseFadeOut.Call(() =>
        {
            Application.Quit();
        });
    }
}