using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropsSpawner : GameProps
{
    [Header("Settings")]
    [SerializeField] Vector2 spawnCooldown;
    [SerializeField] float spawnForce;

    [Space(5)]
    [SerializeField] int objToSpawnOnStart;

    [Header("References")]
    [SerializeField] Transform spawner;
    [SerializeField] Rigidbody2D objToSpawn;

    Queue<Rigidbody2D> objQueue = new();

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    //[Header("Input")]
    //[Header("Output")]
    public override void Launch()
    {
        StartCoroutine(SpawnCooldown());
    }

    private void Awake()
    {
        for (int i = 0; i < objToSpawnOnStart; i++)
        {
            CreateObj();
        }
    }

    IEnumerator SpawnCooldown()
    {
        yield return new WaitForSeconds(Random.Range(spawnCooldown.x, spawnCooldown.y));

        Rigidbody2D obj = GetObj();
        obj.gameObject.SetActive(true);
        obj.transform.position = spawner.position;
        obj.AddForce(spawner.up * spawnForce);

        StartCoroutine(SpawnCooldown());
    }

    Rigidbody2D GetObj()
    {
        if(objQueue.Count <= 0) CreateObj();
        
        return objQueue.Dequeue();
    }
    void CreateObj()
    {
        Rigidbody2D obj = Instantiate(objToSpawn);
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(spawner);

        objQueue.Enqueue(obj);
    }
}