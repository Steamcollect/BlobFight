using System.Collections;
using UnityEngine;

public class BlobMovement : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float idleDistanceMult;
    [SerializeField, Range(0, 1)] float idleDamping;
    [SerializeField] float idleFrequency;

    [Space(10)]
    [SerializeField] float extendDistanceMult;
    [SerializeField, Range(0, 1)] float extendDamping;
    [SerializeField] float extendFrequency;

    [Space(5)]
    [SerializeField] float extendStaminaCostPerSec;
    bool isExtend;

    [Space(10)]
    [SerializeField] float drag;
    [SerializeField] float angularDrag;
    [Space(5)]
    [SerializeField] float moveSpeed;
    [SerializeField] float gravity;

    [Space(5)]
    [SerializeField] float dashForce;
    [SerializeField] float dashCooldown;
    bool canDash = true;

    Vector2 moveInput;

    bool canMove = true;

    [Header("References")]
    [SerializeField] EntityInput entityInput;
    [SerializeField] BlobJoint blobJoint;
    [SerializeField] BlobStamina blobStamina;

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
        blobJoint.SetDrag(drag);
        blobJoint.SetAngularDrag(angularDrag);
        blobJoint.SetGravity(gravity);
        
        blobJoint.MultiplyInitialSpringDistance(idleDistanceMult);
        blobJoint.SetDamping(idleDamping);
        blobJoint.SetFrequency(idleFrequency);
    }

    void ExtendBlob()
    {
        if (!canMove) return;

        if(blobStamina.HaveEnoughStamina(extendStaminaCostPerSec * Time.deltaTime))
        {
            isExtend = true;

            blobJoint.MultiplyInitialSpringDistance(extendDistanceMult);
            blobJoint.SetDamping(extendDamping);
            blobJoint.SetFrequency(extendFrequency);
        }        
    }
    void ShrinkBlob()
    {
        blobJoint.MultiplyInitialSpringDistance(idleDistanceMult);
        blobJoint.SetDamping(idleDamping);
        blobJoint.SetFrequency(idleFrequency);

        isExtend = false;
    }

    void SetInput(Vector2 input)
    {
        moveInput = input;
    }

    void Move()
    {
        blobJoint.Move(Vector2.right * moveInput.x * moveSpeed);
    }
    void Dash()
    {
        if (!canMove || !canDash) return;

        blobJoint.Move(moveInput * dashForce);
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
}