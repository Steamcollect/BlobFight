using UnityEngine;
using UnityEngine.SceneManagement;

public class HideDev : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject content;
    [SerializeField, SceneName] string main;

    private void Awake()
    {
        if(!SceneManager.GetSceneByName(main).isLoaded)
        {
            content.SetActive(true);
        }
    }
}