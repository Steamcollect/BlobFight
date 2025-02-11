using System;
using UnityEngine;

public class BlobMotor : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] SSO_BlobVisuals blobVisuals;

    [Header("References")]
    public BlobJoint joint;
    [SerializeField] BlobVisual visual;
    [SerializeField] BlobHealth health;

    [Space(10)]
    // RSO
    [SerializeField] RSO_BlobInGame rsoBlobInGame;
    // RSF
    // RSP

    //[Header("Input")]

    [Header("Output")]
    [SerializeField] RSE_SpawnBlob rseSpawnBlob;
    [SerializeField] RSE_OnBlobDeath rseOnBlobDeath;

    private void OnEnable()
    {
        health.OnDeath += OnDeath;
    }
    private void OnDisable()
    {
        health.OnDeath -= OnDeath;

        rsoBlobInGame.Value.Remove(this);
    }

    private void Awake()
    {
        if (rsoBlobInGame.Value == null) rsoBlobInGame.Value = new();

        if(rsoBlobInGame.Value.Count >= blobVisuals.blobs.Length)
        {
            Destroy(gameObject);
            return;
        }

        rsoBlobInGame.Value.Add(this);
    }

    private void Start()
    {
        joint.SetupLayer(blobVisuals.blobs[rsoBlobInGame.Value.Count - 1].layer);
        visual.Setup(blobVisuals.blobs[rsoBlobInGame.Value.Count - 1].color);

        Setup();
    }

    void Setup()
    {
        rseSpawnBlob.Call(this);
        Invoke("Enable", .05f);
    }

    public void Enable()
    {
        joint.EnableJoint();
        visual.Show();
    }
    public void Disable()
    {
        joint.DisableJoint();
        visual.Hide();
    }

    public void Spawn(Vector2 position)
    {
        joint.MoveJointsByTransform(position);
        health.Setup();
        Enable();
    }
    void OnDeath()
    {
        Disable();
        rseOnBlobDeath.Call(this);
    }
}