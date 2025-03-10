using System;
using UnityEngine;

public class BlobMovement : MonoBehaviour, IPausable
{
    [Header("Settings")]
    [SerializeField] BlobStatistics shrinkStatistics;
    [SerializeField] BlobStatistics extendStatistics;
    BlobStatistics statistics;

    [Space(15)]
    [SerializeField, Tooltip("Extend stamina cost per second")] float extendStaminaCost;
    [SerializeField] float timeBetweenExtend;
    bool isExtend = false;

    [SerializeField] float slidingGravity;

    Vector2 moveInput;

    bool deathCanMove = true;
    bool pauseCanMove = true;

    [Space(10)]
    [SerializeField] AnimationCurve angleSpeedMultiplierCurve;
    [SerializeField] AnimationCurve angleVelocityMultiplierCurve;
    Vector2 currentGroundNormal;
    float currentGroundAngle;

    [Header("References")]
    [SerializeField] EntityInput input;
    [SerializeField] BlobJoint joint;
    [SerializeField] BlobVisual visual;
    [SerializeField] BlobStamina stamina;
    [SerializeField] BlobTrigger trigger;

    //[Header("Output")]
    public Action onShrink,onExtend;

    private void OnEnable()
    {
        joint.onJointsConnected += SetupJoint;

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

        joint.onJointsConnected -= SetupJoint;

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
                }
                else
                {
                    stamina.RemoveStamina(extendStaminaCost * Time.deltaTime);
                }
            }
        }
        joint.SetCollidersPosOffset(.5f);
    }

    #region Setup
    void SetupJoint()
    {
        statistics = shrinkStatistics;
        SetJointStats();
    }
    void SetJointStats()
    {
        joint.SetDrag(statistics.drag);
        joint.SetGravity(statistics.gravity);

        joint.MultiplyInitialSpringDistance(statistics.distanceMult);
        joint.SetDamping(statistics.damping);
        joint.SetFrequency(statistics.frequency);
        joint.SetMass(statistics.mass);
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
        Debug.DrawLine(joint.GetJointsCenter(), joint.GetJointsCenter() + direction);

        joint.AddForce(direction * moveInput.x * statistics.moveSpeed);
    }

    void ExtendBlob()
    {
        if (!deathCanMove || !stamina.HaveEnoughStamina(extendStaminaCost * Time.deltaTime) || joint.jointsRb[0].bodyType == RigidbodyType2D.Static) return;

        stamina.RemoveStamina(extendStaminaCost * Time.deltaTime);
        stamina.DisableStaminaRecuperation();

        statistics = extendStatistics;
        SetJointStats();

        visual.SetToExtend();

        onExtend?.Invoke();
        isExtend = true;
    }

    void ShrinkBlob()
    {
        if (isExtend && joint.jointsRb[0].bodyType == RigidbodyType2D.Static) return;

        stamina.EnableStaminaRecuperation();

        statistics = shrinkStatistics;
        SetJointStats();

        visual.SetToShrink();
        onShrink?.Invoke();

        isExtend = false;
    }
    #endregion

    #region Collisions
    void OnGroundableEnter(Collision2D collision)
    {
        Vector2 newNormal = collision.GetContact(0).normal;

        float angleDifference = Vector2.Angle(currentGroundNormal, newNormal);
        currentGroundAngle = Mathf.Atan2(newNormal.x, newNormal.y) * Mathf.Rad2Deg;

        float speedFactor = angleVelocityMultiplierCurve.Evaluate(angleDifference);

        Vector2 projectedVelocity = Vector2.Perpendicular(newNormal) * Vector2.Dot(joint.GetVelocity(), Vector2.Perpendicular(newNormal));
        Debug.DrawLine(joint.GetJointsCenter(), joint.GetJointsCenter() + projectedVelocity.normalized, Color.red, 1);

        joint.SetVelocity(projectedVelocity * speedFactor);

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
            joint.SetGravity(slidingGravity);
        }
    }
    void OnSlidableExit(Collision2D collision)
    {
        ExitSlidingState();
    }
    void ExitSlidingState()
    {
        joint.SetGravity(statistics.gravity);
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
    }

    public bool CanMove() { return deathCanMove && pauseCanMove; }
    #endregion

    public bool IsExtend() { return isExtend; }
}