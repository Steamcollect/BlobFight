using System;
using System.Collections;
using UnityEngine;

public class BlobMovement : MonoBehaviour, IPausable
{
    [Header("Settings")]
    [SerializeField] BlobStatistics shrinkStatistics;
    [SerializeField] BlobStatistics extendStatistics;
    BlobStatistics statistics;

    [Space(15)]
    [SerializeField, Tooltip("Extend stamina cost per second")] float extendStaminaCost;
    bool isExtend = false;
    float extendTime;

    [SerializeField] float slidingGravity;

    Vector2 moveInput;

    bool stunImpactCanMove = true;
    bool deathCanMove = true;
    bool pauseCanMove = true;
    bool needShrink = false;

    [Space(10)]
    [SerializeField] AnimationCurve angleSpeedMultiplierCurve;
    [SerializeField] AnimationCurve angleVelocityMultiplierCurve;
    Vector2 currentGroundNormal;
    float currentGroundAngle;

    [Header("References")]
    [SerializeField] EntityInput input;
    [SerializeField] BlobPhysics physics;
    [SerializeField] BlobVisual visual;
    [SerializeField] BlobStamina stamina;
    [SerializeField] BlobTrigger trigger;
    [SerializeField] BlobParticle particle;
    [SerializeField] BlobDash dash;

    //[Header("Output")]
    public Action onShrink,onExtend;

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

        Invoke("LateStart", .1f);
    }
    void LateStart()
    {
        ShrinkBlob();
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
                }
            }
        }

        if (stunImpactCanMove)
        {
            particle.SetExpulseParticleRotation(physics.GetVelocity().normalized);
        }
    }

    #region Setup
    void SetupJoint()
    {
        statistics = shrinkStatistics;
        SetJointStats();
    }
    void SetJointStats()
    {
        if (_RemoveGravity != null) StopCoroutine(_RemoveGravity);

        physics.SetDrag(statistics.drag);
        physics.SetGravity(statistics.gravity);

        physics.ChangeScaleTarget(statistics.scale, statistics.timeToSwap);
        physics.SetMass(statistics.mass);
    }
    void SetInput(Vector2 input)
    {
        if (input.sqrMagnitude > .1f) moveInput = input.normalized;
        else moveInput = Vector2.zero;
    }
    #endregion

    #region ApplyMovement
    void Move()
    {
        Vector2 direction = Vector2.right;

        float angleOffset = angleSpeedMultiplierCurve.Evaluate(Mathf.Abs(currentGroundAngle)) * Mathf.Sign(currentGroundAngle);
        direction = Quaternion.Euler(0, 0, angleOffset) * Vector2.right;
        Debug.DrawLine(physics.GetCenter(), physics.GetCenter() + direction);

        physics.AddForce(direction * moveInput.x * statistics.moveSpeed);
    }

    void ExtendBlob()
    {
        if (!deathCanMove || !pauseCanMove || !stamina.HaveEnoughStamina(extendStaminaCost * Time.deltaTime) || physics.GetRigidbody().bodyType == RigidbodyType2D.Static) return;

        stamina.RemoveStamina(extendStaminaCost * Time.deltaTime);
        stamina.DisableStaminaRecuperation();

        statistics = extendStatistics;
        SetJointStats();

        onExtend?.Invoke();
        isExtend = true;

        visual.Extend();
    }

    void ShrinkBlob()
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
    void OnGroundableEnter(Collision2D collision)
    {
        if (collision.contactCount == 0) return;

        Vector2 newNormal = collision.GetContact(0).normal;

        float angleDifference = Vector2.Angle(currentGroundNormal, newNormal);
        currentGroundAngle = Mathf.Atan2(newNormal.x, newNormal.y) * Mathf.Rad2Deg;

        float speedFactor = angleVelocityMultiplierCurve.Evaluate(angleDifference);

        Vector2 projectedVelocity = Vector2.Perpendicular(newNormal) * Vector2.Dot(physics.GetVelocity(), Vector2.Perpendicular(newNormal));
        Debug.DrawLine(physics.GetCenter(), physics.GetCenter() + projectedVelocity.normalized, Color.red, 1);

        physics.SetVelocity(projectedVelocity * speedFactor);

        currentGroundNormal = newNormal;

        ExitSlidingState();
    }
    void OnGroundableExit(Collision2D collision)
    {
        if (!trigger.IsGrounded())
        {
            currentGroundNormal = Vector2.zero;
            currentGroundAngle = 0;
        }
    }
    
    void OnSlidableEnter(Collision2D collision)
    {
        if (!isExtend)
        {
            Debug.DrawLine(collision.GetContact(0).point, collision.GetContact(0).point + collision.GetContact(0).normal, Color.blue, 1);
            physics.SetGravity(slidingGravity);
        }
    }
    void OnSlidableExit(Collision2D collision)
    {
        ExitSlidingState();
    }
    void ExitSlidingState()
    {
        //physics.SetGravity(statistics.gravity);
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

    Coroutine stunImpactCoroutine;
    public void StunImpact(float delay)
    {
        if (stunImpactCoroutine != null) StopCoroutine(stunImpactCoroutine);
        stunImpactCoroutine = StartCoroutine(StunImpactCooldown(delay));
    }
    IEnumerator StunImpactCooldown(float delay)
    {
        stunImpactCanMove = false;
        particle.EnableExpulseParticle(physics.GetVelocity());
        yield return new WaitForSeconds(delay);
        stunImpactCanMove = true;
        particle.DisableExpulseParticle();
    }

    public float GetExtendTime() { return extendTime; }

    public void RemoveGravity(float delay)
    {
        if (_RemoveGravity != null) StopCoroutine(_RemoveGravity);
        _RemoveGravity = StartCoroutine(RemoveGravityCoroutine(delay));
    }
    Coroutine _RemoveGravity;
    IEnumerator RemoveGravityCoroutine(float delay)
    {
        physics.SetGravity(0);
        yield return new WaitForSeconds(delay);
        physics.SetGravity(statistics.gravity);
    }
}