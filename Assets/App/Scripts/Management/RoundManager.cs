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
    [SerializeField] RSE_LoadNextLevel rseLoadNextLevel;
    [SerializeField] RSE_OnFightEnd rseOnFightEnd;

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
        blobs.Remove(blob);

        if(blobs.Count <= 1)
        {
            rseOnFightEnd.Call();
            rseLoadNextLevel.Call();
        }
    }
}