using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerSelectionPanel : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] int blobRequireToPlay = 1;

    [Header("References")]
    [SerializeField] Button playButton;
    [SerializeField] GameObject blobRequireCountTxt;

    [Space(10)]
    // RSO
    [SerializeField] RSO_BlobInGame rsoBlobInGame;
    // RSF
    // RSP

    [Header("Input")]
    [SerializeField] RSE_SpawnBlob rseSpawnBlob;

    [Header("Output")]
    [SerializeField] RSE_LoadNextLevel rseLoadNextLevel;

    private void OnEnable()
    {
        rseSpawnBlob.action += OnBlobSpawned;
    }
    private void OnDisable()
    {
        rseSpawnBlob.action -= OnBlobSpawned;
    }

    void OnBlobSpawned(BlobJoint blob)
    {
        if(rsoBlobInGame.Value.Count >= blobRequireToPlay)
        {
            playButton.interactable = true;
            blobRequireCountTxt.SetActive(false);
        }
        else
        {
            playButton.interactable = false;
            blobRequireCountTxt.SetActive(true);
        }
    }

    public void PlayButton()
    {
        if (rsoBlobInGame.Value.Count < blobRequireToPlay) return;

        StartCoroutine(Utils.UnloadSceneAsync("MainMenu"));

        rseLoadNextLevel.Call();
    }
}