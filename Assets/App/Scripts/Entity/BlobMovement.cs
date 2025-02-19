using System.Collections;
using UnityEngine;

public class BlobMovement : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] BlobStatistics shrinkStatistics;
    [SerializeField] BlobStatistics extendStatistics;
    BlobStatistics statistics;

    [Space(15)]
    [SerializeField] float extendStaminaCostPerSec;
    bool isExtend;

    [Space(10)]
    [SerializeField] float dashForce;
    [SerializeField] float dashCooldown;
    bool canDash = true;

    Vector2 moveInput;

    bool canMove = true;

    [Header("References")]
    [SerializeField] EntityInput entityInput;
    [SerializeField] BlobJoint blobJoint;
    [SerializeField] BlobStamina blobStamina;
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
        entityInput.compressUpInput -= ShrinkBlob;
        entityInput.moveInput -= SetInput;
        entityInput.dashInput -= Dash;

        blobJoint.onJointsConnected -= SetupJoint;
    }

    private void Start()
    {
        statistics = shrinkStatistics;

        entityInput.compressDownInput += ExtendBlob;
        entityInput.compressUpInput += ShrinkBlob;
        entityInput.dashInput += Dash;

        entityInput.moveInput += SetInput;
    }

    private void Update()
    {
        if (!canMove) return;

        if (isExtend)
        {
            if(!blobStamina.HaveEnoughStamina(extendStaminaCostPerSec * Time.deltaTime))
            {
                ShrinkBlob();
            }
            else
            {
                blobStamina.RemoveStamina(extendStaminaCostPerSec * Time.deltaTime);
            }
        }
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
        if (!canMove) return;

        if(blobStamina.HaveEnoughStamina(extendStaminaCostPerSec * Time.deltaTime))
        {
            isExtend = true;

            statistics = extendStatistics;
            SetJointStats();

            blobVisual.SetToExtend();
        }        
    }
    void ShrinkBlob()
    {
        statistics = shrinkStatistics;
        SetJointStats();

        isExtend = false;

        blobVisual.SetToShrink();
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

    public void EnableMovement()
    {
        canMove = true;
    }
    public void DisableMovement()
    {
        ShrinkBlob();
        canMove = false;
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
}