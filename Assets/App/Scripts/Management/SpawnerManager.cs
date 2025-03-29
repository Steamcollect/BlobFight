using System.Linq;
using UnityEngine;

public class SpawnerManager : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private RSE_SpawnBlob rseSpawnBlob;
    [SerializeField] private RSE_SpawnPoint rseSpawnPoint;

    [Header("Output")]
    [SerializeField] private RSO_BlobInGame rsoBlobInGame;
    [SerializeField] private RSO_Spawnpoints rsoSpawnpoints;

    private void OnEnable()
    {
        rseSpawnBlob.action += SpawnBlob;
        rseSpawnPoint.action += SpawnerBlob;
    }

    private void OnDisable()
    {
        rseSpawnBlob.action -= SpawnBlob;
        rseSpawnPoint.action -= SpawnerBlob;
    }

    private void SpawnerBlob()
    {
        if (rsoBlobInGame.Value.Count == 0) return;

        if (rsoSpawnpoints.Value.Count == 0)
        {
            Debug.LogError("No spawn points available");
            return;
        }

        rsoBlobInGame.Value[0].Spawn(rsoSpawnpoints.Value.GetRandom().position);
        for (int i = 1; i < rsoBlobInGame.Value.Count; i++)
        {
            SpawnBlob(rsoBlobInGame.Value[i]);
        }
    }

    private void SpawnBlob(BlobMotor blob)
    {
        Transform bestSpawn = GetFarthestSpawnPoint(blob);

        if (bestSpawn != null)
        {
            blob.Spawn(bestSpawn.position);
        }
    }

    private Transform GetFarthestSpawnPoint(BlobMotor blob)
    {
        Transform bestSpawn = rsoSpawnpoints.Value
            .OrderByDescending(spawn => rsoBlobInGame.Value
            .Where(b => b != blob)
            .Min(b => Vector2.Distance(spawn.position, b.GetPhysics().GetCenter())))
            .FirstOrDefault();

        return bestSpawn;
    }
}