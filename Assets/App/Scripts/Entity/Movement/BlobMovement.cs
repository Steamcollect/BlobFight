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
    [SerializeField] AnimationCurve angleVelocityMultiplierCurve;
    Vector2 currentNormal;

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
        moveInput = input;
    }
    #endregion

    void Move()
    {
        joint.AddForce(Vector2.right * moveInput.x * statistics.moveSpeed);
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

        float angleDifference = Vector2.Angle(currentNormal, newNormal);

        float speedFactor = angleVelocityMultiplierCurve.Evaluate(angleDifference);

        Vector2 projectedVelocity = Vector2.Perpendicular(newNormal) * Vector2.Dot(joint.GetVelocity(), Vector2.Perpendicular(newNormal));
        Debug.DrawLine(joint.GetJointsCenter(), joint.GetJointsCenter() + projectedVelocity.normalized, Color.red, 1);

        joint.SetVelocity(projectedVelocity * speedFactor);

        currentNormal = newNormal;
    }
    void OnGroundableExit(Collision2D collision)
    {
        if (!trigger.IsGrounded())
        {
            currentNormal = Vector2.zero;
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