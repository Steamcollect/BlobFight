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

    //[Header("References")]

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    [Header("Input")]
    [SerializeField] RSE_LoadNextLevel rseLoadNextLevel;

    [Header("Output")]
    [SerializeField] RSE_FadeIn rseFadeIn;
    [SerializeField] RSE_FadeOut rseFadeOut;
    [SerializeField] RSE_OnFightStart rseOnFightStart;

    private void OnEnable()
    {
        rseLoadNextLevel.action += LoadNextLevelRandomly;
    }
    private void OnDisable()
    {
        rseLoadNextLevel.action -= LoadNextLevelRandomly;
    }

    private void Start()
    {
        StartCoroutine(Utils.LoadSceneAsync(mainMenuName, UnityEngine.SceneManagement.LoadSceneMode.Additive));
        currentLevel = mainMenuName;
    }

    void LoadNextLevelRandomly()
    {
        if (isLoading) return;

        isLoading = true;

        rseFadeOut.Call(() =>
        {
            if (currentLevel != "")
            {
                StartCoroutine(Utils.UnloadSceneAsync(currentLevel));
            }

            if (levels.Count <= 0) levels.AddRange(levelsName);

            int rnd = Random.Range(0, levels.Count);
            currentLevel = levels[rnd];
            levels.RemoveAt(rnd);

            StartCoroutine(Utils.LoadSceneAsync(currentLevel, UnityEngine.SceneManagement.LoadSceneMode.Additive, () =>
            {
                rseFadeIn.Call(() =>
                {
                    rseOnFightStart.Call();
                    isLoading = false;
                });
            }));
        });
        
    }
}