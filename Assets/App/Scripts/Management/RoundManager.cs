using System.Collections.Generic;
using UnityEngine;
public class RoundManager : MonoBehaviour
{
    //[Header("Settings")]

    [Header("References")]

    //[Space(10)]
    // RSO
    [SerializeField] RSO_BlobInGame rsoBlobInGame;
    List<BlobJoint> blobs;
    // RSF
    // RSP

    [Header("Input")]
    [SerializeField] RSE_OnBlobDeath rseOnBlobDeath;

    [Header("Output")]
    [SerializeField] RSE_LoadNextLevel rseLoadNextLevel;

    private void OnEnable()
    {
        rseOnBlobDeath.action += CheckBlobCount;
    }
    private void OnDisable()
    {
        rseOnBlobDeath.action -= CheckBlobCount;
    }

    private void Start()
    {
        blobs = new List<BlobJoint>(rsoBlobInGame.Value);
    }

    void CheckBlobCount(BlobJoint blob)
    {
        blobs.Remove(blob);

        if(blobs.Count <= 1)
        {
            rseLoadNextLevel.Call();
        }
    }
}