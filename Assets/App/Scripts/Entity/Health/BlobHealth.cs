using UnityEngine;
using UnityEngine.InputSystem;

public class BlobHealth : EntityHealth, IPausable
{
    [Header("Settings")]
    [SerializeField] private AnimationCurve percentagePerSpeedOnImpactCurve;
    [SerializeField] private AnimationCurve stunTimePerSpeedOnImpactCurve;
    [SerializeField] private int maxImpactSpeed;
    [SerializeField] private float shakeIntensityOnDeath;
    [SerializeField] private float shakeTimeOnDeath;

    [Header("References")] 
    [SerializeField] private BlobTrigger blobTrigger;
    [SerializeField] private BlobMovement blobMovement;
    [SerializeField] private BlobParticle particle;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private BlobPercentageEffect percentageEffect;

    [Header("Output")]
    [SerializeField] private RSE_CallRumble rseCallRumble;
    [SerializeField] private RSE_CameraShake rseCamShake;

    private float pushBackPercentage = 0;

    private void OnDisable()
    {
        blobTrigger.OnCollisionEnter -= OnEnterCollision;
        onDeath -= OnDeath;
    }

    private void Start()
    {
        Setup();
        Invoke(nameof(LateStart), 0.05f);

    }
    void LateStart()
    {
        blobTrigger.OnCollisionEnter += OnEnterCollision;
        onDeath += OnDeath;
    }

    public void Setup()
    {
        isDead = false;
        currentHealth = maxHealth;

        pushBackPercentage = 0;
    }

    private void OnDeath()
    {
        pushBackPercentage = 0;
        rseCamShake.Call(shakeIntensityOnDeath, shakeTimeOnDeath);
        rseCallRumble.Call(playerInput.user.index, playerInput.currentControlScheme);
    }

    private void OnEnterCollision(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Damagable damagable))
        {
            switch (damagable.GetDamageType())
            {
                case Damagable.DamageType.Damage:
                    TakeDamage(damagable.GetDamage());
                    break;
                
                case Damagable.DamageType.Kill:
                    if (isDead) return;
                    OnDeath();
                    Die();
                    Destroy(collision);
                    break;
                
                case Damagable.DamageType.Destroy:
                    if (isDead) return;
                    OnDeath();
                    Die();
                    Destroy(collision);
                    break;
            }
        }
    }

    public void Pause()
    {
        isInvincible = true;
    }
    public void Resume()
    {
        isInvincible = false;
    }

    public void OnDamageImpact(float speed)
    {
        speed = Mathf.Clamp01(speed / maxImpactSpeed);

        blobMovement.StunImpact(stunTimePerSpeedOnImpactCurve.Evaluate(speed));
        pushBackPercentage += (percentagePerSpeedOnImpactCurve.Evaluate(speed));

        percentageEffect.Setup(pushBackPercentage);
    }
    public void AddPercentage(float percentageGiven)
    {
        pushBackPercentage += percentageGiven;
        percentageEffect.Setup(pushBackPercentage);
    }

    public float GetPercentage() { return 1 + pushBackPercentage; }
}