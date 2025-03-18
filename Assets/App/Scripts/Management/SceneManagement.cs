using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{
    [Header("Settings")]

    [SerializeField] bool isTestScene = false;

    [Space(10)]

    [SerializeField] SceneNameAttribute[] levelsName;

    List<string> levels = new();
    [SerializeField] SceneNameAttribute main;
    [SerializeField] SceneNameAttribute mainMenuName;

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
    [SerializeField] RSE_OnFightStart rseOnFightStart;

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
            rseOnFightStart.Call();
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
        rseFadeOut.Call(() =>
        {
            if (currentLevel != "")
            {
                StartCoroutine(Utils.UnloadSceneAsync(currentLevel));
            }

            if (!isMainMenu)
            {
                if (levels.Count <= 0) levels.AddRange(levelsName.Select(scene => scene.Name));

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
                rseFadeIn.Call(() =>
                {
                    rseEnablePauseAction.Call();
                    rseOnFightStart.Call();
                    isLoading = false;
                });
            }));
        });
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