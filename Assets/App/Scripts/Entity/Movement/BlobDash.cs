using System.Collections;
using UnityEngine;

public class BlobDash : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float dashForce;
    [SerializeField] float dashCooldown;

    [Space(5)]
    [SerializeField] int maxDashCount;
    int dashCount;
    bool canDash = true;
    bool canResetDashCount = true;

    Vector2 dashInput;

    [Header("References")]
    [SerializeField] EntityInput input;
    [SerializeField] BlobJoint joint;
    [SerializeField] BlobTrigger trigger;
    [SerializeField] BlobMovement movement;

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    //[Header("Input")]
    //[Header("Output")]

    private void OnDisable()
    {
        input.moveInput -= SetInput;
        input.dashInput -= Dash;
        trigger.OnGroundedEnter -= ResetDashCount;
        trigger.OnGroundedExit -= ResetDashCount;
    }

    private void Start()
    {
        input.moveInput += SetInput;
        input.dashInput += Dash;
        trigger.OnGroundedEnter += ResetDashCount;
        trigger.OnGroundedExit += ResetDashCount;

        Invoke("LateStart", .1f);
    }

    void LateStart()
    {
        dashCount = maxDashCount;
    }

    void Dash()
    {
        if (!movement.CanMove() || !canDash) return;

        if (dashCount <= 0)
        {
            if (!trigger.IsGrounded()) return;
            else dashCount = maxDashCount;
        }

        dashCount--;
        StartCoroutine(LockResetDashCount());

        joint.ResetVelocity();
        joint.AddForce(dashInput.normalized * dashForce);
        StartCoroutine(DashCooldown());
    }
    IEnumerator DashCooldown()
    {
        canDash = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
    IEnumerator LockResetDashCount()
    {
        canResetDashCount = false;
        yield return new WaitForSeconds(.1f);
        canResetDashCount = true;
    }

    void ResetDashCount(Collision2D collision)
    {
        if(!canResetDashCount) return;
        dashCount = maxDashCount;
    }

    void SetInput(Vector2 input)
    {
        if(input != Vector2.zero) dashInput = input;
    }
}