using System.Collections.Generic;
using UnityEngine;
public class RoundManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] bool modeDev;

    [Header("References")]

    //[Space(10)]
    // RSO
    [SerializeField] RSO_BlobInGame rsoBlobInGame;
    List<BlobMotor> blobs;
    // RSF
    // RSP

    [Header("Input")]
    [SerializeField] RSE_OnBlobDeath rseOnBlobDeath;
    [SerializeField] RSE_OnFightStart rseOnFightStart;
    [SerializeField] private RSE_Message rseMessage;

    [Header("Output")]
    [SerializeField] RSE_OnFightEnd rseOnFightEnd;
    [SerializeField] RSE_AddScore rseAddScore;

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

    void Setup()
    {
        blobs = new List<BlobMotor>(rsoBlobInGame.Value);
    }

    void CheckBlobCount(BlobMotor blob)
    {
        if(blobs != null && blobs.Contains(blob))
        {
            blobs.Remove(blob);

            if (modeDev)
            {
                rseOnFightEnd.Call();
                rseMessage.Call("BLOB ??? WIN!", 1f, Color.black);
            }
            else if (blobs.Count == 1)
            {
                rseAddScore.Call(blobs[0]);
                rseOnFightEnd.Call();
                rseMessage.Call($"BLOB {blobs[0].GetColor().nameColor} WIN!", 1f, blobs[0].GetColor().fillColor);
            }
            else if (blobs.Count < 1)
            {
                rseOnFightEnd.Call();
                rseMessage.Call("BLOB ??? WIN!", 1f, Color.black);
            }
        }
    }
}