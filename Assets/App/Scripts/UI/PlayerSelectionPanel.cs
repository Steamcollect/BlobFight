using UnityEngine;
using UnityEngine.UI;

public class PlayerSelectionPanel : MonoBehaviour
{
    [Header("Settings")]
    //[SerializeField] int blobRequireToPlay = 1;

    [Header("References")]
    [SerializeField] GameObject blobRequireCountTxt;

    [Space(10)]
    // RSO
    [SerializeField] RSO_BlobInGame rsoBlobInGame;
    // RSF
    // RSP

    [Header("Input")]
    [SerializeField] RSE_OnBlobReady rseOnBlobReady;

    [Header("Output")]
    [SerializeField] RSE_LoadNextLevel rseLoadNextLevel;
    [SerializeField] RSE_DisableJoining rseDisableJoining;
    [SerializeField] RSE_OnGameStart rseOnGameStart;

    private void OnEnable()
    {
        rseOnBlobReady.action += OnBlobReady;
    }
    private void OnDisable()
    {
        rseOnBlobReady.action -= OnBlobReady;
    }

    void OnBlobReady()
    {
        for (int i = 0; i < rsoBlobInGame.Value.Count; i++)
        {
            if (!rsoBlobInGame.Value[i].isReady()) return;
        }

        rseDisableJoining.Call();
        rseOnGameStart.Call();
        rseLoadNextLevel.Call();
    }
}