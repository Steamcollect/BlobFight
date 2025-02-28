using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UIElements;

public class BlobMotor : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] SSO_BlobVisuals blobVisuals;
    BlobInitializeStatistic currentStats;
    public BlobInitializeStatistic GetStats() {  return currentStats; }

    private int score;

    [Header("References")]
    [SerializeField] GameObject componentsContent;

    [Space(5)]
    [SerializeField] BlobJoint joint;
    [SerializeField] BlobVisual visual;
    [SerializeField] BlobHealth health;
    [SerializeField] BlobMovement movement;
    [SerializeField] BlobParticle particle;
    [SerializeField] EntityInput input;
    [SerializeField] BlobScore winScore;

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

    [HideInInspector] public Action enableCrown, disableCrown;
    
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

    public void LockInteraction()
    {
        movement.DeathDisableMovement();
    }
    void UnlockInteraction()
    {
        movement.DeathEnableMovement();
    }

    public void Spawn(Vector2 position)
    {
        componentsContent.SetActive(true);

        joint.MoveJointsByTransform(position);
        health.Setup();
        Enable();
    }

    #region Health
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

    public void AddScore()
    {
        winScore.AddScore();
    }
    public int GetScore()
    {
        score = winScore.GetScore();
        return score;
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
    public BlobJoint GetJoint() { return joint; }
    public BlobHealth GetHealth() { return health; }
    public EntityInput GetInput() { return input; }

    public BlobColor GetColor() { return currentStats.color; }
    #endregion
}