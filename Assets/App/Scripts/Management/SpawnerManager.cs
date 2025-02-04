using System.Collections.Generic;
using UnityEngine;
public class SpawnerManager : MonoBehaviour
{
    //[Header("Settings")]

    [Header("References")]
    [SerializeField] BlobJoint[] blobs;
    [SerializeField] Transform[] spawnPoints;

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    //[Header("Input")]
    //[Header("Output")]

    private void Start()
    {
        List<Transform> spawns = new List<Transform>(spawnPoints);
        for (int i = 0; i < blobs.Length; i++)
        {
            int spawnerIndex = Random.Range(0, spawns.Count);
            blobs[i].MoveJointsByTransform(spawnPoints[spawnerIndex].position);
            spawns.RemoveAt(spawnerIndex);
        }
    }
}