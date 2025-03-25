using System.Collections.Generic;
using UnityEngine;
public class RoundManager : MonoBehaviour
{
    //[Header("Settings")]

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
        if(blobs.Contains(blob))
        {
            blobs.Remove(blob);

            if (blobs.Count == 1)
            {
                rseAddScore.Call(blobs[0]);
                rseOnFightEnd.Call();
            }
            else
            {
                rseOnFightEnd.Call();
            }
        }
    }
}