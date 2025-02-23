using System.Collections.Generic;
using UnityEngine;
public class SceneManagement : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] string[] levelsName;
    List<string> levels = new();
	[SerializeField] string mainMenuName;

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

    [Header("Output")]
    [SerializeField] RSE_FadeIn rseFadeIn;
    [SerializeField] RSE_FadeOut rseFadeOut;
    [SerializeField] RSE_OnFightStart rseOnFightStart;

    [Space(5)]
    [SerializeField] RSE_EnablePauseAction rseEnablePauseAction;
    [SerializeField] RSE_DisablePauseAction rseDisablePauseAction;

    private void OnEnable()
    {
        rseLoadNextLevel.action += LoadNextLevelRandomly;
        rseReturnToMainMenu.action += ReturnToMainMenu;
    }
    private void OnDisable()
    {
        rseLoadNextLevel.action -= LoadNextLevelRandomly;
        rseReturnToMainMenu.action -= ReturnToMainMenu;
    }

    private void Start()
    {
        StartCoroutine(Utils.LoadSceneAsync(mainMenuName, UnityEngine.SceneManagement.LoadSceneMode.Additive));
        currentLevel = mainMenuName;
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

        rseFadeOut.Call(() =>
        {
            if (currentLevel != "")
            {
                StartCoroutine(Utils.UnloadSceneAsync(currentLevel));
            }

            if (!isMainMenu)
            {
                if (levels.Count <= 0) levels.AddRange(levelsName);

                int rnd = Random.Range(0, levels.Count);
                currentLevel = levels[rnd];

                levels.RemoveAt(rnd);
            }
            else
            {
                currentLevel = mainMenuName;
            }

            StartCoroutine(Utils.LoadSceneAsync(currentLevel, UnityEngine.SceneManagement.LoadSceneMode.Additive, () =>
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
}