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

    [Space(10)]
    [SerializeField] float dashForce;
    [SerializeField] float dashCooldown;
    bool canDash = true;

    Vector2 moveInput;

    bool deathCanMove = true;
    bool pauseCanMove = true;
    bool canMove
    {
        get
        {
            return deathCanMove && pauseCanMove;
        }
    }

    [Header("References")]
    [SerializeField] EntityInput entityInput;
    [SerializeField] BlobJoint blobJoint;
    [SerializeField] BlobVisual blobVisual;
    [SerializeField] BlobStamina stamina;

    //[Header("Output")]
    public Action onShrink,onExtend;

    private void OnEnable()
    {
        blobJoint.onJointsConnected += SetupJoint;
    }
    private void OnDisable()
    {
        entityInput.compressDownInput -= ExtendBlob;
        entityInput.compressUpInput -= ShrinkBlob;
        entityInput.moveInput -= SetInput;
        entityInput.dashInput -= Dash;

        blobJoint.onJointsConnected -= SetupJoint;
    }

    private void Start()
    {
        entityInput.compressDownInput += ExtendBlob;
        entityInput.compressUpInput += ShrinkBlob;
        entityInput.dashInput += Dash;

        entityInput.moveInput += SetInput;

        Invoke("LateStart", .1f);
    }
    void LateStart()
    {
        ShrinkBlob();
    }

    private void FixedUpdate()
    {
        if (canMove)
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
        blobJoint.SetCollidersPosOffset(.5f);
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

        blobVisual.SetToExtend();

        onExtend?.Invoke();
        isExtend = true;
    }
    void ShrinkBlob()
    {
        statistics = shrinkStatistics;
        SetJointStats();

        blobVisual.SetToShrink();
        onShrink?.Invoke();

        isExtend = false;
    }

    void SetInput(Vector2 input)
    {
        moveInput = input;
    }

    void Move()
    {
        blobJoint.AddForce(Vector2.right * moveInput.x * statistics.moveSpeed);
    }
    void Dash()
    {
        if (!canMove || !canDash) return;

        blobJoint.AddForce(moveInput * dashForce);
        StartCoroutine(DashCooldown());
    }
    IEnumerator DashCooldown()
    {
        canDash = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
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
        blobJoint.SetDrag(statistics.drag);
        blobJoint.SetGravity(statistics.gravity);

        blobJoint.MultiplyInitialSpringDistance(statistics.distanceMult);
        blobJoint.SetDamping(statistics.damping);
        blobJoint.SetFrequency(statistics.frequency);
        blobJoint.SetMass(statistics.mass);
    }

    public void Pause()
    {
        pauseCanMove = false;
    }

    public void Resume()
    {
        pauseCanMove = true;
    }
}