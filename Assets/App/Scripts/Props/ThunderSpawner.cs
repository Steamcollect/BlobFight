using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public class ThunderSpawner : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float delayAfterGameStart;
    //[SerializeField] AnimationCurve curveDifficulty;
    [SerializeField] Vector2 delayBetweenLightning;

    [Header("References")]
    [SerializeField] List<Transform> spawnPoint;
    [SerializeField] GameObject thunderPrefab;

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    [Header("Input")]
    [SerializeField] RSE_OnGameStart RSE_OnGameStart;
    //[Header("Output")]

    private void OnEnable()
    {
        RSE_OnGameStart.action += StartCoroutineDelay;
    }
    private void OnDisable()
    {
        RSE_OnGameStart.action -= StartCoroutineDelay;
    }
    private void StartCoroutineDelay()
    {
        StartCoroutine(StartDelaySpawn());
    }
    IEnumerator StartDelaySpawn()
    {
        yield return new WaitForSeconds(delayAfterGameStart);
        StartCoroutine(SpawnThunder());
    }
    IEnumerator SpawnThunder()
    {
        int rnd = Random.Range(0, spawnPoint.Count);
        Instantiate(thunderPrefab, spawnPoint[rnd]);
        yield return new WaitForSeconds(Random.Range(delayBetweenLightning.x, delayBetweenLightning.y));
        StartCoroutine(SpawnThunder());
    }
}