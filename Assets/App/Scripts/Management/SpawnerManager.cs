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
        blobs[0].MoveJointsByTransform(spawnPoints.GetRandom().position);
        for (int i = 1; i < blobs.Length; i++)
        {
            SpawnBlob(blobs[i]);
        }
    }

    void SpawnBlob(BlobJoint blob)
    {
        Transform bestSpawn = GetFarthestSpawnPoint(blob);
        if (bestSpawn != null)
        {
            blob.MoveJointsByTransform(bestSpawn.position);
        }
    }

    Transform GetFarthestSpawnPoint(BlobJoint excludedBlob)
    {
        Transform bestSpawn = null;
        float maxDistance = float.MinValue;

        foreach (Transform spawn in spawnPoints)
        {
            float minDistanceToBlobs = float.MaxValue;

            foreach (BlobJoint blob in blobs)
            {
                if (blob == excludedBlob) continue;

                float distance = Vector2.Distance(spawn.position, blob.GetJointsCenter());
                if (distance < minDistanceToBlobs)
                {
                    minDistanceToBlobs = distance;
                }
            }

            if (minDistanceToBlobs > maxDistance)
            {
                maxDistance = minDistanceToBlobs;
                bestSpawn = spawn;
            }
        }

        return bestSpawn;
    }
}