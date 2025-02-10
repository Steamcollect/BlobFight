using UnityEngine;
public class Spawnpoint : MonoBehaviour
{
    //[Header("Settings")]

    [Header("References")]

    //[Space(10)]
    // RSO
    [SerializeField] RSO_Spawnpoints rsoSpawnpoints;
    // RSF
    // RSP

    //[Header("Input")]
    //[Header("Output")]

    private void Start()
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