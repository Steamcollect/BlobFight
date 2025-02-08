using System.Collections.Generic;
using UnityEngine;
public class SpawnerManager : MonoBehaviour
{
    //[Header("Settings")]

    [Header("References")]
    
    //[Space(10)]
    // RSO
    [SerializeField] RSO_BlobInGame rsoBlobinGame;
    [SerializeField] RSO_Spawnpoints rsoSpawnPoints;
    // RSF
    // RSP

    //[Header("Input")]
    //[Header("Output")]

    private void Start()
    {
        Invoke("LateStart", .05f);
    }
    void LateStart()
    {
        if (rsoBlobinGame.Value.Count == 0)
        {
            Debug.LogError("You havnt any blob in the scene");
            return;
        }
        if (rsoSpawnPoints.Value.Count == 0)
        {
            Debug.LogError("You havnt any spawnpoint in the scene");
            return;
        }

        rsoBlobinGame.Value[0].MoveJointsByTransform(rsoSpawnPoints.Value.GetRandom().position);
        for (int i = 1; i < rsoBlobinGame.Value.Count; i++)
        {
            SpawnBlob(rsoBlobinGame.Value[i]);
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

        foreach (Transform spawn in rsoSpawnPoints.Value)
        {
            float minDistanceToBlobs = float.MaxValue;

            foreach (BlobJoint blob in rsoBlobinGame.Value)
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