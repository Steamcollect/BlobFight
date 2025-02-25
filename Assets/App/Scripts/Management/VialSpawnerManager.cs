using UnityEngine;
public class VialSpawnerManager : MonoBehaviour
{
    //[Header("Settings")]

    [Header("References")]
    [SerializeField] StartingVial[] vials;

    [Space(10)]
    // RSO
    [SerializeField] RSO_BlobInGame rsoBlobInGame;
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

    void SpawnBlob(BlobMotor blob)
    {
        if (vials.Length < rsoBlobInGame.Value.Count)
        {
            Debug.LogError("There is no enough vial in scene");
            return;
        }

        vials[rsoBlobInGame.Value.Count - 1].Setup(blob);
    }
}