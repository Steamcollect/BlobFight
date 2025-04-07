using System;
using System.Collections;
using UnityEngine;

public class BlobMovement : MonoBehaviour, IPausable
{
    [Header("Settings")]
    [SerializeField] BlobStatistics shrinkStatistics;
    [SerializeField] BlobStatistics extendStatistics;
    [SerializeField] float extendStaminaCost;
    [SerializeField, Range(0,1)] float staminaPercentageRequireToExtend;

    [Space(10)]
    [SerializeField] float slidingGravity;
    [SerializeField] AnimationCurve angleSpeedMultiplierCurve;

    [Header("References")]
    [SerializeField] EntityInput input;
    [SerializeField] BlobPhysics physics;
    [SerializeField] BlobVisual visual;
    [SerializeField] BlobStamina stamina;
    [SerializeField] BlobTrigger trigger;
    [SerializeField] BlobParticle particle;
    [SerializeField] BlobDash dash;

    BlobStatistics statistics = new();
    bool isExtend = false;
    float extendTime = 0;
    Vector2 moveInput = Vector2.zero;
    bool stunImpactCanMove = true;
    bool deathCanMove = true;
    bool pauseCanMove = true;
    bool needShrink = false;
    Vector2 currentGroundNormal = Vector2.zero;
    float currentGroundAngle = 0;
    public Action onShrink,onExtend;
    private Coroutine stunImpactCoroutine;
    Coroutine _RemoveGravity;

    private void OnEnable()
    {
        physics.onJointsConnected += SetupJoint;

        trigger.OnGroundedEnter += OnGroundableEnter;
        trigger.OnGroundedExit += OnGroundableExit;

        trigger.OnSlidableEnter += OnSlidableEnter;
        trigger.OnSlidableExit += OnSlidableExit;
    }

    private void OnDisable()
    {
        input.compressDownInput -= ExtendBlob;
        input.compressUpInput -= ShrinkBlob;
        input.moveInput -= SetInput;

        physics.onJointsConnected -= SetupJoint;

        trigger.OnGroundedEnter -= OnGroundableEnter;
        trigger.OnGroundedExit -= OnGroundableExit;

        trigger.OnSlidableEnter -= OnSlidableEnter;
        trigger.OnSlidableExit -= OnSlidableExit;
    }

    private void Start()
    {
        input.compressDownInput += ExtendBlob;
        input.compressUpInput += ShrinkBlob;

        input.moveInput += SetInput;

        Invoke(nameof(ShrinkBlob), 0.1f);
    }

    private void FixedUpdate()
    {
        if (CanMove())
        {
            Move();
            if (isExtend)
            {
                if (!stamina.HaveEnoughStamina(extendStaminaCost * Time.deltaTime))
                {
                    ShrinkBlob();
                    extendTime = 0;
                }
                else
                {
                    stamina.RemoveStamina(extendStaminaCost * Time.deltaTime);
                    extendTime += Time.deltaTime;

                    if (extendTime > 0.2f && trigger.IsGrounded())
                    {
                        
                    }
                }
            }
        }

        if (stunImpactCanMove)
        {
            particle.SetExpulseParticleRotation(physics.GetVelocity().normalized);
        }
    }

    #region Setup
    private void SetupJoint()
    {
        statistics = shrinkStatistics;
        SetJointStats();
    }

    private void SetJointStats()
    {
        if (_RemoveGravity != null) StopCoroutine(_RemoveGravity);

        physics.SetDrag(statistics.drag);
        physics.SetGravity(statistics.gravity);

        physics.ChangeScaleTarget(statistics.scale, statistics.timeToSwap);
        physics.SetMass(statistics.mass);
    }

    private void SetInput(Vector2 input)
    {
        if (input.sqrMagnitude > .1f) moveInput = input.normalized;
        else moveInput = Vector2.zero;
    }
    #endregion

    #region ApplyMovement
    private void Move()
    {
        float angleOffset = angleSpeedMultiplierCurve.Evaluate(Mathf.Abs(currentGroundAngle)) * Mathf.Sign(currentGroundAngle);
        Vector2 direction = Quaternion.Euler(0, 0, angleOffset) * Vector2.right;
        Debug.DrawLine(physics.GetCenter(), physics.GetCenter() + Vector2.right);

        physics.AddForce(direction * moveInput.x * statistics.moveSpeed);
    }

    private void ExtendBlob()
    {
        if (!deathCanMove || !pauseCanMove || !stamina.HaveEnoughStaminaPercentage(staminaPercentageRequireToExtend) || physics.GetRigidbody().bodyType == RigidbodyType2D.Static) return;

        stamina.RemoveStamina(extendStaminaCost * Time.deltaTime);
        stamina.DisableStaminaRecuperation();

        statistics = extendStatistics;
        SetJointStats();

        onExtend?.Invoke();
        isExtend = true;

        visual.Extend();
    }

    private void ShrinkBlob()
    {
        if (pauseCanMove && isExtend)
        {
            needShrink = true;
        }

        if (!pauseCanMove) return;

        if (isExtend && physics.GetRigidbody().bodyType == RigidbodyType2D.Static) return;

        stamina.EnableStaminaRecuperation();

        statistics = shrinkStatistics;
        SetJointStats();

        onShrink?.Invoke();

        isExtend = false;

        visual.Shrink();
    }
    #endregion

    #region Collisions
    private void OnGroundableEnter(Collision2D collision)
    {
        if (collision.contactCount == 0) return;

        //Vector2 newNormal = collision.GetContact(0).normal;

        //float angleDifference = Vector2.Angle(currentGroundNormal, newNormal);
        //currentGroundAngle = Mathf.Atan2(newNormal.x, newNormal.y) * Mathf.Rad2Deg;

        //float speedFactor = angleVelocityMultiplierCurve.Evaluate(angleDifference);

        //Vector2 projectedVelocity = Vector2.Perpendicular(newNormal) * Vector2.Dot(physics.GetVelocity(), Vector2.Perpendicular(newNormal));
        //Debug.DrawLine(physics.GetCenter(), physics.GetCenter() + projectedVelocity.normalized, Color.red, 1);

        //physics.SetVelocity(projectedVelocity * speedFactor);

        //currentGroundNormal = newNormal;

        ExitSlidingState();
    }

    private void OnGroundableExit(Collision2D collision)
    {
        if (!trigger.IsGrounded())
        {
            currentGroundNormal = Vector2.zero;
            currentGroundAngle = 0;
        }
    }

    private void OnSlidableEnter(Collision2D collision)
    {
        if (!isExtend)
        {
            Debug.DrawLine(collision.GetContact(0).point, collision.GetContact(0).point + collision.GetContact(0).normal, Color.blue, 1);
            physics.SetGravity(slidingGravity);
        }
    }

    private void OnSlidableExit(Collision2D collision)
    {
        ExitSlidingState();
    }

    private void ExitSlidingState()
    {
        physics.SetGravity(statistics.gravity);
    }
    #endregion

    #region State
    public void DeathEnableMovement()
    {
        deathCanMove = true;
    }

    public void DeathDisableMovement()
    {
        ShrinkBlob();
        deathCanMove = false;
    }

    public void Pause()
    {
        pauseCanMove = false;
    }

    public void Resume()
    {
        pauseCanMove = true;

        if (needShrink)
        {
            needShrink = false;
            ShrinkBlob();
        }
    }

    public bool CanMove() { return deathCanMove && pauseCanMove && stunImpactCanMove; }
    #endregion

    public bool IsExtend() { return isExtend; }

    public void StunImpact(float delay)
    {
        if (stunImpactCoroutine != null) StopCoroutine(stunImpactCoroutine);
        stunImpactCoroutine = StartCoroutine(StunImpactCooldown(delay));
    }

    private IEnumerator StunImpactCooldown(float delay)
    {
        stunImpactCanMove = false;
        particle.EnableExpulseParticle(physics.GetVelocity());
        yield return new WaitForSeconds(delay);
        stunImpactCanMove = true;
        particle.DisableExpulseParticle();
    }

    public float GetExtendTime() { return extendTime; }

    public float GetStaminaPercentageRequire() { return staminaPercentageRequireToExtend; }

    public void RemoveGravity(float delay)
    {
        if (_RemoveGravity != null) StopCoroutine(_RemoveGravity);
        _RemoveGravity = StartCoroutine(RemoveGravityCoroutine(delay));
    }
    
    private IEnumerator RemoveGravityCoroutine(float delay)
    {
        physics.SetGravity(0);
        yield return new WaitForSeconds(delay);
        physics.SetGravity(statistics.gravity);
    }
}