using System.Collections.Generic;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private RSE_OnBlobDeath rseOnBlobDeath;
    [SerializeField] private RSE_OnFightStart rseOnFightStart;
    [SerializeField] private RSE_Message rseMessage;

    [Header("Output")]
    [SerializeField] private RSO_BlobInGame rsoBlobInGame;
    [SerializeField] private RSE_OnFightEnd rseOnFightEnd;
    [SerializeField] private RSE_AddScore rseAddScore;

    private List<BlobMotor> blobs = new();

    private void OnEnable()
    {
        rseOnBlobDeath.action += CheckBlobCount;
        rseOnFightStart.action += Setup;
    }

    private void OnDisable()
    {
        rseOnBlobDeath.action -= CheckBlobCount;
        rseOnFightStart.action -= Setup;
    }

    private void Setup()
    {
        blobs = new(rsoBlobInGame.Value);
    }

    private void CheckBlobCount(BlobMotor blob)
    {
        if (blobs != null && blobs.Contains(blob))
        {
            var tempBlob = blob;
            blobs.Remove(tempBlob);

            if (blobs.Count == 0)
            {
                rseOnFightEnd.Call();
                rseMessage.Call($"BLOB {tempBlob.GetColor().nameColor} WIN!", 1f, tempBlob.GetColor().fillColor);
            }
            else if (blobs.Count == 1)
            {
                rseAddScore.Call(blobs[0]);
                rseOnFightEnd.Call();
                rseMessage.Call($"BLOB {blobs[0].GetColor().nameColor} WIN!", 1f, blobs[0].GetColor().fillColor);
            }
        }
    }
}