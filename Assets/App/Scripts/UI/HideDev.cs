using UnityEngine;
using UnityEngine.SceneManagement;

public class HideDev : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject content;
    [SerializeField] SceneReference main;
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