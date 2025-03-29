using UnityEngine;
using UnityEngine.SceneManagement;

public class HideDev : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] SceneReference main;

    [Header("References")]
    [SerializeField] GameObject content;

    [Header("Output")]
    [SerializeField] RSE_OnGameStart rseOnGameStart;

    private void Awake()
    {
        if (!SceneManager.GetSceneByName(main.Name).isLoaded)
        {
            content.SetActive(true);

            StartCoroutine(Utils.Delay(0.1f, () => rseOnGameStart.Call()));
        }
    }
}