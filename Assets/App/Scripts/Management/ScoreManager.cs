using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
public class ScoreManager : MonoBehaviour
{
    //[Header("Settings")]

    [Header("References")]

    [Space(10)]
    // RSO
    [SerializeField] RSO_BlobInGame rsoBlobInGame;
    // RSF
    // RSP

    [Header("Input")]
    [SerializeField] RSE_OnBlobDeath rseOnBlobDeath;
    //[Header("Output")]
    private Dictionary<BlobMotor, int> scoreDictionary = new Dictionary<BlobMotor, int>();
    private BlobMotor blobKey;
    private int max;

    private void OnEnable()
    {
        rseOnBlobDeath.action += UpdateCrownOwner;
    }
    private void OnDisable()
    {
        rseOnBlobDeath.action -= UpdateCrownOwner;
    }
    private void Start()
    {
        GetScore();
        GetScoreMax();

    }
    private void GetScore()
    {
        for (int i = 0; rsoBlobInGame.Value.Count > i; i++)
        {
            blobKey = rsoBlobInGame.Value[i];
            scoreDictionary.Add(blobKey, blobKey.GetScore());
        }
    }
    private void GetScoreMax()
    {
        max = scoreDictionary.Values.Max();
        blobKey = scoreDictionary.FirstOrDefault(x => x.Value == max).Key;
        for (int i = 0; rsoBlobInGame.Value.Count > i; i++)
        {
            BlobMotor blob = rsoBlobInGame.Value[i];
            blob.DisableCrown();
        }
        if (max != 0)
        {
            blobKey.EnableCrown();
        }
    }
    private void UpdateCrownOwner(BlobMotor blob)
    {
        Debug.Log("Dead: " +blob.name+" "+ blob.IsAlive());
        blob.DisableCrown();
        BlobMotor bestPlayer = scoreDictionary
                    .Where(p => p.Key.IsAlive())
                    .OrderByDescending(p => p.Value)
                    .FirstOrDefault().Key;
        Debug.Log(bestPlayer.IsAlive());
        Debug.Log(bestPlayer.name);
        if (bestPlayer != null)
        {
            bestPlayer.EnableCrown();
        }
    }
}