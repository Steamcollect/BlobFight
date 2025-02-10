using UnityEngine;
public class WrapperInitializer : MonoBehaviour
{
    [Header("RSO")]
    [SerializeField] RSO_BlobInGame rsoBlobInGame;
    [SerializeField] RSO_Spawnpoints rsoSpawnpoints;

    private void Awake()
    {
        WrapperInitializer wrapperInitializer = FindFirstObjectByType<WrapperInitializer>();
        if (wrapperInitializer != null && wrapperInitializer != this)
        {
            Destroy(gameObject);
            return;
        }

        rsoBlobInGame.Value = new();
        rsoSpawnpoints.Value = new();
    }
}