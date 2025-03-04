using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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
    [SerializeField] RSE_OnFightStart rseOnFightStart;
    [SerializeField] RSE_UpdateCrownColor rseUpdateCrownColor;
    //[Header("Output")]
    private Dictionary<BlobMotor, int> scoreDictionary = new Dictionary<BlobMotor, int>();
    private BlobMotor blobKey;
    private int max;
    private bool crownIsHere;
    private int counterEgality;
    private void OnEnable()
    {
        rseOnBlobDeath.action += UpdateCrownOwner;
        rseOnFightStart.action += SetUp;
    }
    private void OnDisable()
    {
        rseOnBlobDeath.action -= UpdateCrownOwner;
        rseOnFightStart.action -= SetUp;
    }
    private void SetUp()
    {
        crownIsHere = false;
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
        counterEgality = 0;
        max = scoreDictionary.Values.Max();
        blobKey = scoreDictionary.FirstOrDefault(x => x.Value == max).Key;
        for (int i = 0; rsoBlobInGame.Value.Count > i; i++)
        {
            BlobMotor blob = rsoBlobInGame.Value[i];
            blob.DisableCrown();
        }
        if (max != 0)
        {
            crownIsHere = true;
            foreach (KeyValuePair<BlobMotor, int> blobScore in scoreDictionary)
            {
                if (blobScore.Value == max)
                {
                    counterEgality++;
                    blobScore.Key.EnableCrown();
                }
            }
            if (counterEgality == 1)
            {
                rseUpdateCrownColor.Call(true);
            }
            else
            {
                rseUpdateCrownColor.Call(false);
            }
        }
    }
    private void UpdateCrownOwner(BlobMotor blob)
    {
        counterEgality = 0;
        blob.DisableCrown();
        BlobMotor bestPlayer = scoreDictionary
                    .Where(p => p.Key.IsAlive())
                    .OrderByDescending(p => p.Value)
                    .FirstOrDefault().Key;
        if (bestPlayer != null && crownIsHere)
        {
            foreach (KeyValuePair<BlobMotor, int> blobScore in scoreDictionary)
            {
                if (blobScore.Value == bestPlayer.GetScore())
                {
                    counterEgality++;
                    blobScore.Key.EnableCrown();
                }
            }
            if (counterEgality == 1)
            {
                rseUpdateCrownColor.Call(true);
            }
            else
            {
                rseUpdateCrownColor.Call(false);
            }
        }
    }
}