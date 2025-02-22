using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlobMotor : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] SSO_BlobVisuals blobVisuals;
    BlobInitializeStatistic currentStats;

    [Header("References")]
    public BlobJoint joint;
    [SerializeField] BlobVisual visual;
    public BlobHealth health;
    [SerializeField] BlobMovement movement;
    [SerializeField] BlobParticle particle;

    IPausable[] pausables;

    [Space(10)]
    // RSO
    [SerializeField] RSO_BlobInGame rsoBlobInGame;
    // RSF
    // RSP

    [Header("Input")]
    [SerializeField] RSE_OnFightStart rseOnFightStart;
    [SerializeField] RSE_OnFightEnd rseOnFightEnd;

    [Header("Output")]
    [SerializeField] RSE_SpawnBlob rseSpawnBlob;
    [SerializeField] RSE_OnBlobDeath rseOnBlobDeath;

    [Space(5)]
    [SerializeField] RSE_OnPause rseOnPause;
    [SerializeField] RSE_OnResume rseOnResume;

    private void OnEnable()
    {
        rseOnFightStart.action += UnlockInteraction;
        rseOnFightEnd.action += LockInteraction;

        rseOnPause.action += OnGamePause;
        rseOnResume.action += OnGameResume;

        health.onDeath += OnDeath;
        health.onDestroy += OnDestroyed;
    }
    private void OnDisable()
    {
        rseOnFightStart.action -= UnlockInteraction;
        rseOnFightEnd.action -= LockInteraction;

        rseOnPause.action -= OnGamePause;
        rseOnResume.action -= OnGameResume;

        health.onDeath -= OnDeath;
        health.onDestroy -= OnDestroyed;

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
        pausables = GetComponentsInChildren<IPausable>();

        currentStats = blobVisuals.blobs[rsoBlobInGame.Value.Count - 1];
        joint.SetupLayer(currentStats.layer);
        visual.Setup(currentStats.color);

        Setup();
        UnlockInteraction();
    }

    void Setup()
    {
        rseSpawnBlob.Call(this);
    }

    void Enable()
    {
        joint.EnableJoint();
        visual.Show();
    }
    void Disable()
    {
        joint.DisableJoint();
        visual.Hide();

        LockInteraction();
    }

    void LockInteraction()
    {
        movement.DeathDisableMovement();
    }
    void UnlockInteraction()
    {
        movement.DeathEnableMovement();
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

    public bool IsAlive() { return !health.IsDead(); }

    void OnGamePause()
    {
        foreach (IPausable pausable in pausables)
        {
            pausable.Pause();
        }
    }
    void OnGameResume()
    {
        foreach (IPausable pausable in pausables)
        {
            pausable.Resume();
        }
    }
}