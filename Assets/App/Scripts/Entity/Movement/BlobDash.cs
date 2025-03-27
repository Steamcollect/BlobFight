using System;
using System.Collections;
using UnityEngine;

public class BlobDash : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float dashForce;
    [SerializeField] float dashCooldown;
    [SerializeField] float removeGravityTime;

    [Space(5)]
    [SerializeField] int maxDashCount;
    int dashCount;
    bool canDash = true;
    bool canResetDashCount = true;

    Vector2 dashInput = Vector2.up;

    [Header("References")]
    [SerializeField] EntityInput input;
    [SerializeField] BlobPhysics physics;
    [SerializeField] BlobTrigger trigger;
    [SerializeField] BlobMovement movement;
    [SerializeField] BlobParticle particle;

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    //[Header("Input")]
    //[Header("Output")]
    public Action OnDash;

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

        Invoke("LateStart", .1f);
    }

    void LateStart()
    {
        dashCount = maxDashCount;
    }

    void Dash()
    {
        if (!movement.CanMove() || !canDash) return;

        if (trigger.IsGrounded()
            || trigger.IsSliding()
            || trigger.IsInWind())
            dashCount = maxDashCount;

        if(dashCount <= 0) return;

        dashCount--;

        if (!trigger.IsGrounded() && !trigger.IsSliding())
            particle.DustDashParticle(physics.GetButtomPosition(), dashInput.normalized);
        else
            foreach (var contact in trigger.GetAllContacts())
                particle.DustParticle(contact.point, contact.normal);

            StartCoroutine(LockResetDashCount());

        physics.ResetVelocity();
        physics.AddForce(dashInput.normalized * dashForce);
        StartCoroutine(DashCooldown());

        movement.RemoveGravity(removeGravityTime);

        OnDash?.Invoke();
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

    void ResetDashCountOnCollision(Collision2D coll) { ResetDashCount(); }
    void ResetDashCount()
    {
        if(!canResetDashCount) return;
        dashCount = maxDashCount;
    }

    void SetInput(Vector2 input)
    {
        if (input != Vector2.zero) dashInput = input;
        else dashInput = Vector2.up;
    }
}