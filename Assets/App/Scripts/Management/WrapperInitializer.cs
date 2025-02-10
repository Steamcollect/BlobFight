using UnityEngine;
public class WrapperInitializer : MonoBehaviour
{
    [Header("RSO")]
    [SerializeField] RSO_BlobInGame rsoBlobInGame;
    [SerializeField] RSO_Spawnpoints rsoSpawnpoints;

    private void Awake()
    {
        rsoBlobInGame.Value = new();
        rsoSpawnpoints.Value = new();
    }
}