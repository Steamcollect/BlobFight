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
    [SerializeField] BlobParticle particle;

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
        health.OnDestroy += OnDestroyed;
    }
    private void OnDisable()
    {
        health.OnDeath -= OnDeath;
        health.OnDestroy -= OnDestroyed;

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
        movement.DisableMovement();
    }

    public void Spawn(Vector2 position)
    {
        joint.MoveJointsByTransform(position);
        health.Setup();
        Enable();
    }
    void OnDeath()
    {
        particle.DeathParticle(joint.GetJointsCenter(), currentStats.color);

        Disable();
        rseOnBlobDeath.Call(this);
    }
    void OnDestroyed(ContactPoint2D contact)
    {
        particle.DestroyParticle(contact, currentStats.color);

        Disable();
        rseOnBlobDeath.Call(this);
    }

    public BlobColor GetColor()
    {
        return currentStats.color;
    }
}