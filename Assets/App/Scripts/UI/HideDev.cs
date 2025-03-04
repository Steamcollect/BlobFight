using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HideDev : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject content;
    [SerializeField, SceneName] string main;
    [SerializeField] RSE_OnGameStart rseOnGameStart;

    private void Awake()
    {
        string scenePath = AssetDatabase.GUIDToAssetPath(main);
        string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);

        if (!SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            content.SetActive(true);
            rseOnGameStart.Call();
        }
    }
}