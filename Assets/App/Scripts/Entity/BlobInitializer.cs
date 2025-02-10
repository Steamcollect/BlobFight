using UnityEngine;

public class BlobInitializer : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] BlobInitializeStatistic[] blob;

    [Header("References")]
    [SerializeField] BlobJoint joint;
    [SerializeField] BlobVisual visual;

    [Space(10)]
    // RSO
    [SerializeField] RSO_BlobInGame rsoBlobInGame;
    // RSF
    // RSP

    //[Header("Input")]
    [Header("Output")]
    [SerializeField] RSE_SpawnBlob rseSpawnBlob;

    private void Awake()
    {
        rsoBlobInGame.Value.Add(joint);
    }

    private void Start()
    {
        visual.Setup(blob[rsoBlobInGame.Value.Count - 1].color);

        Invoke("LateStart", .05f);
    }

    void LateStart()
    {
        joint.SetupLayer(blob[rsoBlobInGame.Value.Count - 1].layer);

        rseSpawnBlob.Call(joint);
    }
}

[System.Serializable]
public struct BlobInitializeStatistic
{
    public BlobColor color;
    public LayerMask layer;
}

[System.Serializable]
public struct BlobColor
{
    public Color fillColor, outlineColor;
}