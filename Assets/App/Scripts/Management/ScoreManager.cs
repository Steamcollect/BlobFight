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
    [SerializeField] RSE_OnFightStart rseOnFightStart;
    [SerializeField] RSE_UpdateCrownVisual rseUpdateCrownVisual;
    [SerializeField] RSE_AddScore rseAddScore;

    //[Header("Output")]
    private SerializableDictionary<BlobMotor, int> scoreDictionary = new();
    private BlobMotor blobKey;
    private int max;
    private bool crownIsHere;
    private int counterEgality;

    private void OnEnable()
    {
        rseOnBlobDeath.action += UpdateCrownOwner;
        rseOnGameStart.action += SetUp;
        rseOnFightStart.action += GetScoreMax;
        rseAddScore.action += AddScore;
    }

    private void OnDisable()
    {
        rseOnBlobDeath.action -= UpdateCrownOwner;
        rseOnGameStart.action -= SetUp;
        rseOnFightStart.action -= GetScoreMax;
        rseAddScore.action -= AddScore;
    }

    private void SetUp()
    {
        crownIsHere = false;
        scoreDictionary.Clear();

        for (int i = 0; rsoBlobInGame.Value.Count > i; i++)
        {
            blobKey = rsoBlobInGame.Value[i];
            scoreDictionary.Add(blobKey, 0);
        }
    }

    private void GetScoreMax()
    {
        counterEgality = 0;

        if (!scoreDictionary.Any())
        {
            return;
        }

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
            counterEgality = scoreDictionary.Count(pair => pair.Value == max);

            foreach (var blobScore in scoreDictionary.Where(pair => pair.Value == max))
            {
                blobScore.Key.EnableCrown();
            }

            rseUpdateCrownVisual.Call(counterEgality == 1);
        }
    }

    private void UpdateCrownOwner(BlobMotor blob)
    {
        bool anyBlobAlive = scoreDictionary.Keys.Any(blob => blob.IsAlive());

        if (anyBlobAlive)
        {
            counterEgality = 0;
            blob.DisableCrown();

            int highestScore = scoreDictionary
                        .Where(p => p.Key.IsAlive())
                        .Max(p => p.Value);

            List<BlobMotor> bestPlayers = scoreDictionary
                .Where(p => p.Key.IsAlive() && p.Value == highestScore)
                .Select(p => p.Key)
                .ToList();

            if (bestPlayers.Count > 0 && crownIsHere)
            {
                foreach (BlobMotor player in bestPlayers)
                {
                    counterEgality++;
                    player.EnableCrown();
                }

                rseUpdateCrownVisual.Call(counterEgality == 1);
            }
        }
    }

    private void AddScore(BlobMotor blob)
    {
        if (scoreDictionary.ContainsKey(blob))
        {
            scoreDictionary[blob] += 1;
        }
    }
}