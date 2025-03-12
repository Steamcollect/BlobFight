using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public class ThunderSpawner : GameProps
{
    [Header("Settings")]
    [SerializeField] float delayAfterGameStart;
    //[SerializeField] AnimationCurve curveDifficulty;
    [SerializeField] Vector2 delayBetweenLightning;
    [Space(5)]
    [SerializeField] int objToSpawnOnStart;
    [Header("References")]
    [SerializeField] List<Transform> spawnPoint;
    [SerializeField] ThunderProps thunderPrefab;

    Queue<ThunderProps> thunderQueue = new();

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    //[Header("Input")]
    //[Header("Output")]
    private void Awake()
    {
        for (int i = 0; i < objToSpawnOnStart; i++)
        {
            CreateThunderObj();
        }
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
        yield return new WaitForSeconds(Random.Range(delayBetweenLightning.x, delayBetweenLightning.y));
        int rnd = Random.Range(0, spawnPoint.Count);
        ThunderProps thunder = GetThunderObj();
        thunder.gameObject.SetActive(true);
        thunder.transform.position = spawnPoint[rnd].position;
        StartCoroutine(SpawnThunder());
    }
    ThunderProps GetThunderObj()
    {
        if (thunderQueue.Count <= 0) CreateThunderObj();

        return thunderQueue.Dequeue();
    }
    private void CreateThunderObj()
    {
        ThunderProps obj = Instantiate(thunderPrefab);
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(spawnPoint[0]);
        obj.onEndAction += QueueThunder;
        thunderQueue.Enqueue(obj);
    }
    private void QueueThunder(ThunderProps thunder)
    {
        thunderQueue.Enqueue(thunder);
    }

    public override void Launch()
    {
        StartCoroutineDelay();
    }
}