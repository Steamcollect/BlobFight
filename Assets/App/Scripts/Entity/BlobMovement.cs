using System.Collections;
using UnityEngine;

public class BlobMovement : MonoBehaviour, IPausable
{
    [Header("Settings")]
    [SerializeField] BlobStatistics shrinkStatistics;
    [SerializeField] BlobStatistics extendStatistics;
    BlobStatistics statistics;

    [Space(15)]
    [SerializeField] float extendTime;
    [SerializeField] float extendCooldown;
    bool canExtend = true;

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

    class MySpringJoint
    {
        public SpringJoint2D spring;
        public float distance;

        public MySpringJoint(SpringJoint2D spring, float distance)
        {
            this.spring = spring;
            this.distance = distance;
        }
    }

    private void OnEnable()
    {
        blobJoint.onJointsConnected += SetupJoint;
    }
    private void OnDisable()
    {
        entityInput.compressDownInput -= ExtendBlob;
        entityInput.moveInput -= SetInput;
        entityInput.dashInput -= Dash;

        blobJoint.onJointsConnected -= SetupJoint;
    }

    private void Start()
    {
        entityInput.compressDownInput += ExtendBlob;
        entityInput.dashInput += Dash;

        entityInput.moveInput += SetInput;

        Invoke("LateStart", .1f);
    }
    void LateStart()
    {
        statistics = shrinkStatistics;
        SetJointStats();
    }

    private void FixedUpdate()
    {
        if (canMove)
        {
            Move();
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
        if (!deathCanMove || !canExtend) return;

        statistics = extendStatistics;
        SetJointStats();

        blobVisual.SetToExtend();

        StartCoroutine(ExtendTime());
    }
    void ShrinkBlob()
    {
        statistics = shrinkStatistics;
        SetJointStats();

        blobVisual.SetToShrink();
    }
    IEnumerator ExtendTime()
    {
        yield return new WaitForSeconds(extendTime);
        ShrinkBlob();
        StartCoroutine(ExtendCooldown());
    }
    IEnumerator ExtendCooldown()
    {
        canExtend = false;
        yield return new WaitForSeconds(extendCooldown);
        canExtend = true;
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