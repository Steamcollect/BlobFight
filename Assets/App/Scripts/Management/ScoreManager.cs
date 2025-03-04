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
    [SerializeField] RSE_OnGameStart rseOnGameStart;
    [SerializeField] RSE_UpdateCrownColor rseUpdateCrownColor;

    //[Header("Output")]
    private SerializableDictionary<BlobMotor, int> scoreDictionary = new SerializableDictionary<BlobMotor, int>();
    private BlobMotor blobKey;
    private int max;
    private bool crownIsHere;
    private int counterEgality;

    private void OnEnable()
    {
        rseOnBlobDeath.action += UpdateCrownOwner;
        rseOnGameStart.action += SetUp;
    }
    private void OnDisable()
    {
        rseOnBlobDeath.action -= UpdateCrownOwner;
        rseOnGameStart.action -= SetUp;
    }
    private void SetUp()
    {
        crownIsHere = false;
        GetScores();
        GetScoreMax();
    }
    private void GetScores()
    {
        for (int i = 0; rsoBlobInGame.Value.Count > i; i++)
        {
            blobKey = rsoBlobInGame.Value[i];
            scoreDictionary.Dictionary.Add(blobKey, blobKey.GetScore());
        }
    }
    private void GetScoreMax()
    {
        counterEgality = 0;
        max = scoreDictionary.Dictionary.Values.Max();
        blobKey = scoreDictionary.Dictionary.FirstOrDefault(x => x.Value == max).Key;
        for (int i = 0; rsoBlobInGame.Value.Count > i; i++)
        {
            BlobMotor blob = rsoBlobInGame.Value[i];
            blob.DisableCrown();
        }
        if (max != 0)
        {
            crownIsHere = true;
            foreach (KeyValuePair<BlobMotor, int> blobScore in scoreDictionary.Dictionary)
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
        BlobMotor bestPlayer = scoreDictionary.Dictionary
                    .Where(p => p.Key.IsAlive())
                    .OrderByDescending(p => p.Value)
                    .FirstOrDefault().Key;
        if (bestPlayer != null && crownIsHere)
        {
            foreach (KeyValuePair<BlobMotor, int> blobScore in scoreDictionary.Dictionary)
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