using UnityEngine;

public class PlayerSelectionPanel : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] RSE_OnBlobReady rseOnBlobReady;

    [Header("Output")]
    [SerializeField] RSO_BlobInGame rsoBlobInGame;
    [SerializeField] RSE_DisableJoining rseDisableJoining;
    [SerializeField] RSE_OnGameStart rseOnGameStart;
    [SerializeField] RSE_Transit rseTransit;
    [SerializeField] RSE_Message rseMessage;
    [SerializeField] SSO_ListFightText listFightText;

    private void OnEnable()
    {
        rseOnBlobReady.action += OnBlobReady;
    }

    private void OnDisable()
    {
        rseOnBlobReady.action -= OnBlobReady;
    }

    private void OnBlobReady()
    {
        if (rsoBlobInGame.Value.TrueForAll(blob => blob.IsReady()))
        {
            rseDisableJoining.Call();
            rseOnGameStart.Call();
            rseTransit.Call();
            rseMessage.Call("GAME START!", 1f, listFightText.colorMessage);
        }
    }
}