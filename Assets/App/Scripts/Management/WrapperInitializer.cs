using UnityEngine;
public class WrapperInitializer : MonoBehaviour
{
    [Header("RSO")]
    [SerializeField] RSO_BlobInGame rsoblobInGame;
    [SerializeField] RSO_Spawnpoints rsoSpawnpoints;

    private void Awake()
    {
        rsoblobInGame.Value = new();
        rsoSpawnpoints.Value = new();
    }
}