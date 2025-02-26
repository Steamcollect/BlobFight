using System;
using System.Collections;
using UnityEngine;

public class BlobMovement : MonoBehaviour, IPausable
{
    [Header("Settings")]
    [SerializeField] BlobStatistics shrinkStatistics;
    [SerializeField] BlobStatistics extendStatistics;
    BlobStatistics statistics;

    [SerializeField, Tooltip("Extend stamina cost per second")] float extendStaminaCost;
    bool isExtend = false;

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
    }
    private void OnDisable()
    {
        input.compressDownInput -= ExtendBlob;
        input.compressUpInput -= ShrinkBlob;
        input.moveInput -= SetInput;

        joint.onJointsConnected -= SetupJoint;

        trigger.OnGroundedEnter -= OnGroundableEnter;
        trigger.OnGroundedExit -= OnGroundableExit;
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
        moveInput = input.normalized;
    }
    #endregion

    void Move()
    {
        float angleOffset = angleSpeedMultiplierCurve.Evaluate(Mathf.Abs(currentGroundAngle)) * Mathf.Sign(currentGroundAngle);
        Vector2 direction = Quaternion.Euler(0, 0, angleOffset) * Vector2.right;
        Debug.DrawLine(joint.GetJointsCenter(), joint.GetJointsCenter() + direction);

        joint.AddForce(direction * moveInput.x * statistics.moveSpeed);

        //joint.AddForce(Vector2.right * moveInput.x * statistics.moveSpeed);
    }

    void ExtendBlob()
    {
        if (!deathCanMove || !stamina.HaveEnoughStamina(extendStaminaCost * Time.deltaTime)) return;

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
        stamina.EnableStaminaRecuperation();

        statistics = shrinkStatistics;
        SetJointStats();

        visual.SetToShrink();
        onShrink?.Invoke();

        isExtend = false;
    }

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
    }
    void OnGroundableExit(Collision2D collision)
    {
        if (!trigger.IsGrounded())
        {
            currentGroundNormal = Vector2.zero;
        }
    }

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
}