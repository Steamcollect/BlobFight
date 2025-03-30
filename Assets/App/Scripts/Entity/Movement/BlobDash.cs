using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class BlobDash : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float dashForce;
    [SerializeField] private float dashCooldown;
    [SerializeField] private float removeGravityTime;
    [SerializeField] private int maxDashCount;

    [Header("References")]
    [SerializeField] private EntityInput input;
    [SerializeField] private BlobPhysics physics;
    [SerializeField] private BlobTrigger trigger;
    [SerializeField] private BlobMovement movement;
    [SerializeField] private BlobParticle particle;

    public Action OnDash;
    private int dashCount = 0;
    private bool canDash = true;
    private bool canResetDashCount = true;
    private Vector2 dashInput = Vector2.up;

    private void OnDisable()
    {
        input.moveInput -= SetInput;
        input.dashInput -= Dash;

        trigger.OnGroundedEnter -= ResetDashCountOnCollision;
        trigger.OnSlidableEnter -= ResetDashCountOnCollision;
        trigger._OnWindEnter -= ResetDashCount;
    }

    private void Start()
    {
        input.moveInput += SetInput;
        input.dashInput += Dash;

        trigger.OnGroundedEnter += ResetDashCountOnCollision;
        trigger.OnSlidableEnter += ResetDashCountOnCollision;
        trigger._OnWindEnter += ResetDashCount;

        dashCount = maxDashCount;
    }

    private void Dash()
    {
        if (!movement.CanMove() || !canDash || movement.IsExtend()) return;

        if (trigger.IsGrounded()
            || trigger.IsSliding()
            || trigger.IsInWind())
            dashCount = maxDashCount;

        if(dashCount <= 0) return;

        dashCount--;

        if (trigger.IsGrounded() || trigger.IsSliding())
            trigger.GetAllContacts().ToList().ForEach(contact => particle.DustParticle(contact.point, contact.normal));
        else
            particle.DustDashParticle(physics.GetButtomPosition(), dashInput.normalized);

        StartCoroutine(LockResetDashCount());

        physics.ResetVelocity();
        physics.AddForce(dashInput.normalized * dashForce);
        StartCoroutine(DashCooldown());

        movement.RemoveGravity(removeGravityTime);

        OnDash?.Invoke();
    }

    private IEnumerator DashCooldown()
    {
        canDash = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    private IEnumerator LockResetDashCount()
    {
        canResetDashCount = false;
        yield return new WaitForSeconds(.1f);
        canResetDashCount = true;
    }

    private void ResetDashCountOnCollision(Collision2D coll) { ResetDashCount(); }

    private void ResetDashCount()
    {
        if(!canResetDashCount) return;
        dashCount = maxDashCount;
    }

    private void SetInput(Vector2 input)
    {
        if (input != Vector2.zero) dashInput = input;
        else dashInput = Vector2.up;
    }
}