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

    [Header("References")]
    [SerializeField] EntityInput input;
    [SerializeField] BlobJoint joint;
    [SerializeField] BlobVisual visual;
    [SerializeField] BlobStamina stamina;

    //[Header("Output")]
    public Action onShrink,onExtend;

    private void OnEnable()
    {
        joint.onJointsConnected += SetupJoint;
    }
    private void OnDisable()
    {
        input.compressDownInput -= ExtendBlob;
        input.compressUpInput -= ShrinkBlob;
        input.moveInput -= SetInput;

        joint.onJointsConnected -= SetupJoint;
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

    void SetupJoint()
    {
        statistics = shrinkStatistics;
        SetJointStats();
    }

    void ExtendBlob()
    {
        if (!deathCanMove || !stamina.HaveEnoughStamina(extendStaminaCost * Time.deltaTime)) return;

        stamina.RemoveStamina(extendStaminaCost * Time.deltaTime);

        statistics = extendStatistics;
        SetJointStats();

        visual.SetToExtend();

        onExtend?.Invoke();
        isExtend = true;
    }
    void ShrinkBlob()
    {
        statistics = shrinkStatistics;
        SetJointStats();

        visual.SetToShrink();
        onShrink?.Invoke();

        isExtend = false;
    }

    void SetInput(Vector2 input)
    {
        moveInput = input;
    }

    void Move()
    {
        joint.AddForce(Vector2.right * moveInput.x * statistics.moveSpeed);
    }

    public void DeathEnableMovement()
    {
        deathCanMove = true;
    }
    public void DeathDisableMovement()
    {
        ShrinkBlob();
        deathCanMove = false;
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

    public void Pause()
    {
        pauseCanMove = false;
    }

    public void Resume()
    {
        pauseCanMove = true;
    }

    public bool CanMove() { return deathCanMove && pauseCanMove; }
}