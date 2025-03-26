using System;
using System.Collections.Generic;
using UnityEngine;

public class BlobMotor : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] SSO_BlobVisuals blobVisuals;
    [SerializeField] bool menu;
    BlobInitializeStatistic currentStats;

    [Header("References")]
    [SerializeField] GameObject componentsContent;

    [Space(5)]
    [SerializeField] BlobPhysics physics;
    [SerializeField] BlobVisual visual;
    [SerializeField] BlobHealth health;
    [SerializeField] BlobMovement movement;
    [SerializeField] BlobParticle particle;
    [SerializeField] EntityInput input;
    [SerializeField] BlobTrigger trigger;
    [SerializeField] BlobCombat combat;
    [SerializeField] BlobAudio blobAudio;

	[Space(5)]
    [SerializeField] BlobReadyValidationPanel preparationPanel;

    IPausable[] pausables;

    [Space(10)]
    // RSO
    [SerializeField] RSO_BlobInGame rsoBlobInGame;
    // RSF
    // RSP

    [Header("Input")]
    [SerializeField] RSE_OnFightStart rseOnFightStart;
    [SerializeField] RSE_OnFightEnd rseOnFightEnd;
    [SerializeField] RSE_OnGameStart rseOnGameStart;

    [Header("Output")]
    [SerializeField] RSE_SpawnBlob rseSpawnBlob;
    [SerializeField] RSE_OnBlobDeath rseOnBlobDeath;
    [SerializeField] RSE_TogglePause rseTogglePause;

    [Space(5)]
    [SerializeField] RSE_OnPause rseOnPause;
    [SerializeField] RSE_OnResume rseOnResume;

    public Action enableCrown, disableCrown;

    #region Setup
    private void OnEnable()
    {
        rseOnFightStart.action += UnlockInteraction;
        rseOnFightEnd.action += LockInteraction;
        rseOnGameStart.action += LockInteraction;

        rseOnPause.action += OnGamePause;
        rseOnResume.action += OnGameResume;

        input.pauseInput += rseTogglePause.Call;

        health.onDeath += OnDeath;
        health.onDestroy += OnDestroyed;
    }
    private void OnDisable()
    {
        rseOnFightStart.action -= UnlockInteraction;
        rseOnFightEnd.action -= LockInteraction;
        rseOnGameStart.action -= LockInteraction;

        rseOnPause.action -= OnGamePause;
        rseOnResume.action -= OnGameResume;

        input.pauseInput -= rseTogglePause.Call;

        health.onDeath -= OnDeath;
        health.onDestroy -= OnDestroyed;

        rsoBlobInGame.Value.Remove(this);
    }

    private void Awake()
    {
        componentsContent.SetActive(false);

        if(rsoBlobInGame.Value.Count >= blobVisuals.blobs.Length)
        {
            Destroy(gameObject);
            return;
        }
    }
    private void Start()
    {
        rsoBlobInGame.Value.Add(this);

        pausables = GetComponentsInChildren<IPausable>(true);

        currentStats = blobVisuals.blobs[rsoBlobInGame.Value.Count - 1];
        gameObject.name = currentStats.blobName;

        visual.SetColor(currentStats.color.fillColor);

        if (menu)
        {
            Setup();
            UnlockInteraction();
        }

        Invoke("LateStart", .1f);
    }

    public void Setup()
    {
        rseSpawnBlob.Call(this);
    }

    void LateStart()
    {
        physics.SetupLayer(currentStats.layer);
        trigger.SetLayerToExclude(currentStats.layer);
        combat.SetLayer(currentStats.layer);
    }

    #endregion

    void Enable()
    {
        physics.Enable();
        visual.Show();
    }
    void Disable()
    {
        physics.Disable();
        visual.Hide();

        LockInteraction();
    }

    public void LockInteraction()
    {
        movement.DeathDisableMovement();
        physics.Disable();
    }
    void UnlockInteraction()
    {
        movement.DeathEnableMovement();
        physics.Enable();
    }

    public void Spawn(Vector2 position)
    {
        componentsContent.SetActive(true);

        physics.MoveTo(position);
        health.Setup();
        Enable();
    }

    #region Health
    void OnDeath()
    {
        particle.DeathParticle(physics.GetCenter(), currentStats.color);
        Disable();
        rseOnBlobDeath.Call(this);
    }
    void OnDestroyed(ContactPoint2D contact)
    {
        particle.DestroyParticle(contact, currentStats.color);
        Disable();
        rseOnBlobDeath.Call(this);
    }

    public bool IsAlive() { return !health.IsDead(); }
    #endregion

    #region GameState
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
    #endregion

    #region Score
    public void EnableCrown()
    {
        enableCrown?.Invoke();
    }
    public void DisableCrown()
    {
        disableCrown?.Invoke();
    }
    #endregion

    #region PreparationState
    public void OnJoined()
    {
        preparationPanel.gameObject.SetActive(true);
    }
    public bool IsReady() { return preparationPanel.IsReady(); }
    #endregion

    #region Getter
    public BlobInitializeStatistic GetStats() { return currentStats; }
    public BlobPhysics GetPhysics() { return physics; }
    public BlobHealth GetHealth() { return health; }
    public EntityInput GetInput() { return input; }
    public BlobMovement GetMovement() { return movement; }
    public BlobTrigger GetTrigger() { return trigger; }
    public BlobCombat GetCombat() { return combat; }
    public BlobAudio GetAudio() { return blobAudio; }

    public BlobColor GetColor() { return currentStats.color; }
    #endregion
}