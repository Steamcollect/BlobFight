using System.Collections.Generic;
using UnityEngine;
public class SceneManagement : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] string[] levelsName;
    List<string> levels = new();

    string currentLevel = "";

    //[Header("References")]

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    [Header("Input")]
    [SerializeField] RSE_LoadNextLevel rseLoadNextLevel;

    //[Header("Output")]

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
        StartCoroutine(Utils.LoadSceneAsync("MainMenu", UnityEngine.SceneManagement.LoadSceneMode.Additive));
    }

    void LoadNextLevelRandomly()
    {
        if(currentLevel != "")
        {
            StartCoroutine(Utils.UnloadSceneAsync(currentLevel));
        }

        if (levels.Count <= 0) levels.AddRange(levelsName);

        int rnd = Random.Range(0, levels.Count);
        currentLevel = levels[rnd];
        levels.RemoveAt(rnd);
        
        StartCoroutine(Utils.LoadSceneAsync(currentLevel, UnityEngine.SceneManagement.LoadSceneMode.Additive));
    }
}