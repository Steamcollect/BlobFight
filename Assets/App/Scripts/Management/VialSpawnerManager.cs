using UnityEngine;

public class VialSpawnerManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private VialSpawner[] vials;

    [Header("Input")]
    [SerializeField] private RSE_SpawnBlob rseSpawnBlob;

    [Header("Output")]
    [SerializeField] private RSO_BlobInGame rsoBlobInGame;

    private void OnEnable()
    {
        rseSpawnBlob.action += SpawnBlob;
    }

    private void OnDisable()
    {
        rseSpawnBlob.action -= SpawnBlob;
    }

    private void SpawnBlob(BlobMotor blob)
    {
        if (vials.Length < rsoBlobInGame.Value.Count)
        {
            Debug.LogError("There is no enough vial in scene");
            return;
        }

        vials[rsoBlobInGame.Value.Count - 1].Setup(blob);
    }
}