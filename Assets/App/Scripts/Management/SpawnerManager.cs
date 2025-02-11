using System.Collections.Generic;
using UnityEngine;
public class SpawnerManager : MonoBehaviour
{
    //[Header("Settings")]

    [Header("References")]
    
    //[Space(10)]
    // RSO
    [SerializeField] RSO_BlobInGame rsoBlobInGame;
    [SerializeField] RSO_Spawnpoints rsoSpawnpoints;
    // RSF
    // RSP

    [Header("Input")]
    [SerializeField] RSE_SpawnBlob rseSpawnBlob;

    //[Header("Output")]

    private void OnEnable()
    {
        rseSpawnBlob.action += SpawnBlob;
    }
    private void OnDisable()
    {
         rseSpawnBlob.action -= SpawnBlob;
    }

    private void Start()
    {
        Invoke("LateStart", .05f);
    }
    void LateStart()
    {
        if (rsoBlobInGame.Value.Count == 0) return;
        if (rsoSpawnpoints.Value.Count == 0)
        {
            Debug.LogError("You havnt any spawnpoint in the scene");
            return;
        }

        rsoBlobInGame.Value[0].Spawn(rsoSpawnpoints.Value.GetRandom().position);
        for (int i = 1; i < rsoBlobInGame.Value.Count; i++)
        {
            SpawnBlob(rsoBlobInGame.Value[i]);
        }
    }

    void SpawnBlob(BlobMotor blob)
    {
        Transform bestSpawn = GetFarthestSpawnPoint(blob);
        if (bestSpawn != null)
        {
            blob.Spawn(bestSpawn.position);
        }
    }

    Transform GetFarthestSpawnPoint(BlobMotor excludedBlob)
    {
        Transform bestSpawn = null;
        float maxDistance = float.MinValue;

        foreach (Transform spawn in rsoSpawnpoints.Value)
        {
            float minDistanceToBlobs = float.MaxValue;

            foreach (BlobMotor blob in rsoBlobInGame.Value)
            {
                if (blob == excludedBlob) continue;

                float distance = Vector2.Distance(spawn.position, blob.joint.GetJointsCenter());
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