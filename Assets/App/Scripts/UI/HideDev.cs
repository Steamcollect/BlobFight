using UnityEngine;
using UnityEngine.SceneManagement;

public class HideDev : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject content;
    [SerializeField] SceneNameAttribute main;
    [SerializeField] RSE_OnGameStart rseOnGameStart;

    private void Awake()
    {
        if (!SceneManager.GetSceneByName(main.Name).isLoaded)
        {
            content.SetActive(true);
            rseOnGameStart.Call();
        }
    }
}