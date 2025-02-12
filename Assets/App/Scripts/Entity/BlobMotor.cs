using System;
using UnityEngine;

public class BlobMotor : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] SSO_BlobVisuals blobVisuals;
    BlobInitializeStatistic currentStats;

    [Header("References")]
    public BlobJoint joint;
    [SerializeField] BlobVisual visual;
    [SerializeField] BlobHealth health;
    [SerializeField] BlobStamina stamina;
    [SerializeField] BlobMovement movement;

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
        health.OnDestroy += Disable;
    }
    private void OnDisable()
    {
        health.OnDeath -= OnDeath;
        health.OnDeath -= Disable;

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
        currentStats = blobVisuals.blobs[rsoBlobInGame.Value.Count - 1];
        joint.SetupLayer(currentStats.layer);
        visual.Setup(currentStats.color);

        Setup();
    }

    void Setup()
    {
        stamina.Setup();

        rseSpawnBlob.Call(this);
        Invoke("Enable", .05f);
    }

    public void Enable()
    {
        movement.EnableMovement();
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
        movement.DisableMovement();

        rseOnBlobDeath.Call(this);
    }

    public BlobColor GetColor()
    {
        return currentStats.color;
    }
}