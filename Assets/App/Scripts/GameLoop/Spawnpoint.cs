using UnityEngine;

public class Spawnpoint : MonoBehaviour
{
    [Header("Output")]
    [SerializeField] RSO_Spawnpoints rsoSpawnpoints;
    
    private void OnEnable()
    {
        if (rsoSpawnpoints.Value == null)
        {
            rsoSpawnpoints.Value = new();
        }

        rsoSpawnpoints.Value.Add(transform);
    }

    private void OnDisable()
    {
        rsoSpawnpoints.Value.Remove(transform);
    }
}