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
    [Space(10)]
    [SerializeField, TagName] string lavaTag;
    [SerializeField, TagName] string voidTag;
    [SerializeField, TagName] string brumbleTag;
    [SerializeField, TagName] string laserTag;

    [Header("References")]
    [SerializeField] private BlobTrigger blobTrigger;
    [SerializeField] private BlobMovement blobMovement;
    [SerializeField] private BlobParticle particle;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private BlobPercentageEffect percentageEffect;
    [SerializeField] private BlobPhysics physics;
    [SerializeField] private BlobAudio blobAudio;

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
            if (damagable.GetDamageType() == Damagable.DamageType.Damage)
            {
                AddPercentage(damagable.GetDamage());
                physics.AddForce(collision.GetContact(0).normal * damagable.GetPushBackForce() * GetPercentage());
                if (damagable.CompareTag(lavaTag))
                {
                    blobAudio.PlayHitFromLavaClip();
                }
                else if (damagable.CompareTag(brumbleTag))
                {
                    blobAudio.PlayHitFromBrumbleClip();
                }
                else if (damagable.CompareTag(laserTag))
                {
                    blobAudio.PlayHitFromLaserClip();
                }
            }
            else if (damagable.GetDamageType() == Damagable.DamageType.Kill || damagable.GetDamageType() == Damagable.DamageType.Destroy)
            {
                if (isDead) return;
                OnDeath();

                if (damagable.GetDamageType() == Damagable.DamageType.Kill) Die();
                else Destroy(collision);

                if (damagable.CompareTag(lavaTag))
                {
                    blobAudio.PlayDeathFromLavaClip();
                }
                else if (damagable.CompareTag(voidTag))
                {
                    blobAudio.PlayDeathFromVoidClip();
                }
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