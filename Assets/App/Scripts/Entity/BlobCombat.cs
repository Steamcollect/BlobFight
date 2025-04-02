using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlobCombat : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float pushBackForce;
    [SerializeField] private float returnPushBackForce;
    [SerializeField] private float extendForceMultiplier;
    [SerializeField] private float percentageMultiplier;
    [SerializeField] private float speedMultToPushExtendBlob;
    [SerializeField] private float parryMaxTime;
    [SerializeField] private float paryForceMultiplier;
    [SerializeField] private float minSpeedAtImpact;
    [SerializeField] private float zoomDelay;

    [Header("References")]
    [SerializeField] private BlobTrigger trigger;
    [SerializeField] private BlobPhysics physics;
    [SerializeField] private BlobMovement movement;
    [SerializeField] private BlobParticle particle;

    [Header("Output")]
    [SerializeField] private RSE_CameraZoom rseCameraZoom;
    [SerializeField] private RSE_OnPause rseOnPause;

    private LayerMask currentLayer;
    public Action<float> OnHitBlob;
    private bool canFight = true;
    private List<BlobMotor> blobsTouch = new();

    private void OnEnable()
    {
        trigger.OnBlobCollisionEnter += OnBlobCollisionEnter;
    }

    private void OnDisable()
    {
        trigger.OnBlobCollisionEnter -= OnBlobCollisionEnter;
    }

    private void OnBlobCollisionEnter(BlobMotor blobTouch, Collision2D collision)
    {
        if (blobsTouch.Contains(blobTouch) || !canFight) return;

        var blobMovement = blobTouch.GetMovement();
        var blobPhysics = blobTouch.GetPhysics();
        var blobHealth = blobTouch.GetHealth();

        float speed = physics.lastVelocity.sqrMagnitude;

        float blobTouchSpeed = blobPhysics.lastVelocity.sqrMagnitude;
        
        Vector2 propulsionDir = (blobPhysics.GetCenter() - physics.GetCenter()).normalized;

        Vector2 impactVelocity = Vector2.zero;
        Vector2 impactForce = Vector2.zero;

        if (movement.IsExtend() && movement.GetExtendTime() < parryMaxTime && speed < blobTouchSpeed)
        {
            print("Parry");
            rseCameraZoom.Call(physics.transform.position, zoomDelay);
            rseOnPause.Call();

            impactVelocity = propulsionDir * blobTouchSpeed;
            impactForce = impactVelocity * paryForceMultiplier * (blobHealth.GetPercentage() * percentageMultiplier);

            particle.ParryParticle(collision.GetContact(0).point, propulsionDir);
        }
        else if(blobMovement.IsExtend() && blobMovement.GetExtendTime() < parryMaxTime && speed > blobTouchSpeed)
        {
            return;
        }
        else if (!blobMovement.IsExtend() && movement.IsExtend() && (blobTouchSpeed * speedMultToPushExtendBlob) < speed)
        {
            impactVelocity = propulsionDir * Mathf.Max(speed, minSpeedAtImpact);
            impactForce = impactVelocity * extendForceMultiplier * (blobHealth.GetPercentage() * percentageMultiplier);

            particle.DoHitParticle(collision.GetContact(0).point, propulsionDir, impactForce.sqrMagnitude);
        }
        else if (!movement.IsExtend() && blobMovement.IsExtend() && (speed * speedMultToPushExtendBlob) > blobTouchSpeed)
        {
            impactVelocity = propulsionDir * Mathf.Max(speed, minSpeedAtImpact);
            impactForce = impactVelocity * extendForceMultiplier * (blobHealth.GetPercentage() * percentageMultiplier);

            blobTouch.GetTrigger().ExludeLayer(currentLayer, .1f);

            particle.DoHitParticle(collision.GetContact(0).point, propulsionDir, impactForce.sqrMagnitude);
        }
        else if ((!movement.IsExtend() && !blobMovement.IsExtend()) || (movement.IsExtend() && blobMovement.IsExtend()))
        {
            if (speed < blobTouchSpeed) return; // Do not add force in this case

            impactVelocity = propulsionDir * speed;
            impactForce = impactVelocity * (blobHealth.GetPercentage() * percentageMultiplier);

            particle.DoHitParticle(collision.GetContact(0).point, propulsionDir, impactForce.sqrMagnitude);
        }
        else return;

        // Set new velocity
        blobPhysics.ResetVelocity();
        physics.ResetVelocity();

        Debug.DrawLine(blobPhysics.GetCenter(), blobPhysics.GetCenter() + impactForce, Color.black, 1f);

        blobPhysics.AddForce(impactForce * pushBackForce);
        physics.AddForce(-impactVelocity * returnPushBackForce);

        // Set health
        blobHealth.OnDamageImpact(impactForce.sqrMagnitude);

        OnHitBlob.Invoke(0);

        // Set cooldowns
        StartCoroutine(ImpactCooldown(blobTouch));
        StartCoroutine(blobTouch.GetCombat().CanFightCooldown());
    }

    private IEnumerator ImpactCooldown(BlobMotor blobTouch)
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