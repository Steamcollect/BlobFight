using System;
using UnityEngine;

public class BlobMotor : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool menu;

    [Header("References")]
    [SerializeField] private GameObject componentsContent;
    [SerializeField] private BlobPhysics physics;
    [SerializeField] private BlobVisual visual;
    [SerializeField] private BlobHealth health;
    [SerializeField] private BlobMovement movement;
    [SerializeField] private BlobParticle particle;
    [SerializeField] private EntityInput input;
    [SerializeField] private BlobTrigger trigger;
    [SerializeField] private BlobCombat combat;
    [SerializeField] private BlobAudio blobAudio;
    [SerializeField] private BlobReadyValidationPanel preparationPanel;

    [Header("Input")]
    [SerializeField] private RSE_OnFightStart rseOnFightStart;
    [SerializeField] private RSE_OnFightEnd rseOnFightEnd;
    [SerializeField] private RSE_OnGameStart rseOnGameStart;
    [SerializeField] private RSE_OnPause rseOnPause;
    [SerializeField] private RSE_OnResume rseOnResume;

    [Header("Output")]
    [SerializeField] private SSO_BlobVisuals blobVisuals;
    [SerializeField] private RSE_SpawnBlob rseSpawnBlob;
    [SerializeField] private RSE_OnBlobDeath rseOnBlobDeath;
    [SerializeField] private RSE_TogglePause rseTogglePause;
    [SerializeField] private RSO_BlobInGame rsoBlobInGame;

    public Action enableCrown;
    public Action disableCrown;
    private BlobInitializeStatistic currentStats;
    private IPausable[] pausables;

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

        trigger.setParent += (Transform t) => transform.SetParent(t);
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

        Invoke(nameof(LateStart), 0.1f);
    }

    public void Setup()
    {
        rseSpawnBlob.Call(this);
    }

    private void LateStart()
    {
        physics.SetupLayer(currentStats.layer);
        trigger.SetLayerToExclude(currentStats.layer);
        combat.SetLayer(currentStats.layer);
    }

    #endregion

    private void Enable()
    {
        physics.Enable();
        visual.Show();
    }

    private void Disable()
    {
        physics.Disable();
        visual.Hide();

        LockInteraction();
    }

    public void LockInteraction()
    {
        trigger.lockInteraction = true;
        movement.DeathDisableMovement();
        physics.Disable();
        trigger.ResetTouchs();
    }

    private void UnlockInteraction()
    {
        trigger.lockInteraction = false;
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
    private void OnDeath()
    {
        particle.DeathParticle(physics.GetCenter(), currentStats.color);
        Disable();
        rseOnBlobDeath.Call(this);
    }

    private void OnDestroyed(ContactPoint2D contact)
    {
        particle.DestroyParticle(contact, currentStats.color);
        Disable();
        rseOnBlobDeath.Call(this);
    }

    public bool IsAlive() { return !health.IsDead(); }
    #endregion

    #region GameState
    private void OnGamePause()
    {
        foreach (IPausable pausable in pausables)
        {
            pausable.Pause();
        }
    }

    private void OnGameResume()
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