using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlobCombat : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float pushBackForce;
    [SerializeField] float returnPushBackForce;
    [SerializeField] float extendForceMultiplier;

    [SerializeField] float percentageMultiplier;

    [Space(5)]
    [SerializeField] float speedMultToPushExtendBlob = .4f;

    [Space(5)]
    [SerializeField] float parryMaxTime;
    [SerializeField] float paryForceMultiplier;

    bool canFight = true;

    [Space(10)]
    [SerializeField] float minSpeedAtImpact;

    [Header("References")]
    [SerializeField] BlobTrigger trigger;
    [SerializeField] BlobPhysics physics;
    [SerializeField] BlobMovement movement;
    [SerializeField] BlobParticle particle;

    LayerMask currentLayer;

    List<BlobMotor> blobsTouch = new();
    public Action<float> OnHitBlob;

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    //[Header("Input")]
    //[Header("Output")]

    private void OnEnable()
    {
        trigger.OnBlobCollisionEnter += OnBlobCollisionEnter;
    }
    private void OnDisable()
    {
        trigger.OnBlobCollisionEnter -= OnBlobCollisionEnter;
    }

    void OnBlobCollisionEnter(BlobMotor blobTouch, Collision2D collision)
    {
        if (blobsTouch.Contains(blobTouch) || !canFight) return;

        float speed = physics.lastVelocity.sqrMagnitude;

        float blobTouchSpeed = blobTouch.GetPhysics().lastVelocity.sqrMagnitude;
        
        Vector2 propulsionDir = (blobTouch.GetPhysics().GetCenter() - physics.GetCenter()).normalized;

        Vector2 impactVelocity = Vector2.zero;
        Vector2 impactForce = Vector2.zero;

        if (movement.IsExtend() && movement.GetExtendTime() < parryMaxTime && speed < blobTouchSpeed)
        {
            print("Parry");

            impactVelocity = propulsionDir * blobTouchSpeed;
            impactForce = impactVelocity * paryForceMultiplier * (blobTouch.GetHealth().GetPercentage() * percentageMultiplier);
        }
        else if(blobTouch.GetMovement().IsExtend() && blobTouch.GetMovement().GetExtendTime() < parryMaxTime && speed > blobTouchSpeed)
        {
            return;
        }
        else if (!blobTouch.GetMovement().IsExtend() && movement.IsExtend() && (blobTouchSpeed * speedMultToPushExtendBlob) < speed)
        {
            impactVelocity = propulsionDir * (speed < minSpeedAtImpact ? minSpeedAtImpact : speed);
            impactForce = impactVelocity * extendForceMultiplier * (blobTouch.GetHealth().GetPercentage() * percentageMultiplier);
        }
        else if (!movement.IsExtend() && blobTouch.GetMovement().IsExtend() && (speed * speedMultToPushExtendBlob) > blobTouchSpeed)
        {
            impactVelocity = propulsionDir * (speed < minSpeedAtImpact ? minSpeedAtImpact : speed);
            impactForce = impactVelocity * extendForceMultiplier * (blobTouch.GetHealth().GetPercentage() * percentageMultiplier);

            blobTouch.GetTrigger().ExludeLayer(currentLayer, .1f);
        }
        else if ((!movement.IsExtend() && !blobTouch.GetMovement().IsExtend()) || (movement.IsExtend() && blobTouch.GetMovement().IsExtend()))
        {
            if (speed < blobTouchSpeed) return; // Do not add force in this case

            impactVelocity = propulsionDir * speed;
            impactForce = impactVelocity * (blobTouch.GetHealth().GetPercentage() * percentageMultiplier);
        }
        else return;

        // Set new velocity
        blobTouch.GetPhysics().ResetVelocity();
        physics.ResetVelocity();

        Debug.DrawLine(blobTouch.GetPhysics().GetCenter(), blobTouch.GetPhysics().GetCenter() + impactForce, Color.black, 1f);

        blobTouch.GetPhysics().AddForce(impactForce * pushBackForce);
        physics.AddForce(-impactVelocity * returnPushBackForce);

        // Set health
        blobTouch.GetHealth().OnDamageImpact(impactForce.sqrMagnitude);

        particle.DoHitParticle(collision.GetContact(0).point, propulsionDir, impactForce.sqrMagnitude);
        OnHitBlob.Invoke(0);

        // Set cooldowns
        StartCoroutine(ImpactCooldown(blobTouch));
        StartCoroutine(blobTouch.GetCombat().CanFightCooldown());
    }
    IEnumerator ImpactCooldown(BlobMotor blobTouch)
    {
        blobsTouch.Add(blobTouch);
        yield return new WaitForSeconds(.1f);
        blobsTouch.Remove(blobTouch);
    }
    public IEnumerator CanFightCooldown()
    {
        canFight = false;
        yield return new WaitForSeconds(.1f);
        canFight = true;
    }

    public void SetLayer(LayerMask layer)
    {
        currentLayer = layer;
    }
}