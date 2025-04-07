using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlobCombat : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float pushBackForce;
    [SerializeField] private float returnPushBackForce;

    [Space(5)]
    [SerializeField] private float extendForceMultiplier;
    [SerializeField] private float percentageMultiplier;

    [Space(5)]
    [SerializeField] private float speedMultToPushExtendBlob;

    [Space(10)]
    [SerializeField] private float parryMaxTime;
    [SerializeField] private float paryForceMultiplier;
    [SerializeField] float speedRequireToParry;

    [Space(5)]
    [SerializeField] private float minSpeedAtImpact;
    
    [Space(5)]
    [SerializeField] private float zoomDelayAtParry;
    [SerializeField] private float expulseForceParrymultiplier;

    [Header("References")]
    [SerializeField] BlobMotor motor;

    [Header("Output")]
    [SerializeField] private RSE_CameraZoom rseCameraZoom;
    [SerializeField] private RSE_OnPause rseOnPause;

    private LayerMask currentLayer;
    private bool canFight = true;
    private List<BlobMotor> blobsTouch = new();

    private void OnEnable()
    {
        motor.GetTrigger().OnBlobCollisionEnter += OnBlobCollisionEnter;
    }

    private void OnDisable()
    {
        motor.GetTrigger().OnBlobCollisionEnter -= OnBlobCollisionEnter;
    }

    private void OnBlobCollisionEnter(BlobMotor blobTouch, Collision2D collision)
    {
        if (blobsTouch.Contains(blobTouch) || !canFight) return;

        var blobMovement = blobTouch.GetMovement();
        var blobPhysics = blobTouch.GetPhysics();
        var blobHealth = blobTouch.GetHealth();

        float speed = motor.GetPhysics().lastVelocity.sqrMagnitude;

        float blobTouchSpeed = blobPhysics.lastVelocity.sqrMagnitude;
        
        Vector2 propulsionDir = (blobPhysics.GetCenter() - motor.GetPhysics().GetCenter()).normalized;

        Vector2 impactVelocity = Vector2.zero;
        Vector2 impactForce = Vector2.zero;

        if (speed < blobTouchSpeed) return; // Do not add force in this case

        if (motor.GetMovement().IsExtend()
            && motor.GetMovement().GetExtendTime() < parryMaxTime
            && speed < blobTouchSpeed
            && blobTouchSpeed > speedRequireToParry)
        {
            print(blobTouchSpeed);
            StartCoroutine(ParryImpact(propulsionDir * expulseForceParrymultiplier + Vector2.up * expulseForceParrymultiplier, blobTouchSpeed, collision, blobTouch));
            return;
        }
        else if(blobMovement.IsExtend() 
            && blobMovement.GetExtendTime() < parryMaxTime 
            && speed > blobTouchSpeed
            && speed > speedRequireToParry)
        {
            return;
        }
        else if (!blobMovement.IsExtend() 
            && motor.GetMovement().IsExtend() 
            && (blobTouchSpeed * speedMultToPushExtendBlob) < speed)
        {
            impactVelocity = propulsionDir * Mathf.Max(speed, minSpeedAtImpact);
            impactForce = impactVelocity * extendForceMultiplier * (blobHealth.GetPercentage() * percentageMultiplier);

            motor.GetParticle().DoHitParticle(collision.GetContact(0).point, propulsionDir, impactForce.sqrMagnitude);
        }
        else if (!motor.GetMovement().IsExtend() 
            && blobMovement.IsExtend() 
            && (speed * speedMultToPushExtendBlob) > blobTouchSpeed)
        {
            impactVelocity = propulsionDir * Mathf.Max(speed, minSpeedAtImpact);
            impactForce = impactVelocity * extendForceMultiplier * (blobHealth.GetPercentage() * percentageMultiplier);

            blobTouch.GetTrigger().ExludeLayer(currentLayer, .1f);

            motor.GetParticle().DoHitParticle(collision.GetContact(0).point, propulsionDir, impactForce.sqrMagnitude);
        }
        else if ((!motor.GetMovement().IsExtend() 
            && !blobMovement.IsExtend()) || (motor.GetMovement().IsExtend() 
            && blobMovement.IsExtend()))
        {
            impactVelocity = propulsionDir * speed;
            impactForce = impactVelocity * (blobHealth.GetPercentage() * percentageMultiplier);

            motor.GetParticle().DoHitParticle(collision.GetContact(0).point, propulsionDir, impactForce.sqrMagnitude);
        }
        else return;

        // Set new velocity
        blobPhysics.ResetVelocity();
        motor.GetPhysics().ResetVelocity();

        blobPhysics.AddForce(impactForce * pushBackForce);
        motor.GetPhysics().AddForce(-impactVelocity * returnPushBackForce);

        // Set health
        blobHealth.OnDamageImpact(impactForce.sqrMagnitude);

        // Set cooldowns
        StartCoroutine(ImpactCooldown(blobTouch));
        StartCoroutine(blobTouch.GetCombat().CanFightCooldown());
    }

    IEnumerator ParryImpact(Vector2 propulsionDir, float blobTouchSpeed, Collision2D collision, BlobMotor blobTouch)
    {
        rseCameraZoom.Call(motor.GetPhysics().transform.position, zoomDelayAtParry);
        rseOnPause.Call();

        yield return new WaitForSeconds(.2f);

        Vector2 impactVelocity = propulsionDir * blobTouchSpeed;
        Vector2 impactForce = impactVelocity * paryForceMultiplier * (blobTouch.GetHealth().GetPercentage() * percentageMultiplier);

        motor.GetParticle().ParryParticle(collision.GetContact(0).point, propulsionDir);
        motor.GetAudio().PlayParrySound();

        yield return new WaitForSeconds(.6f);

        blobTouch.GetAudio().PlayHitFromParrySound();

        // Set new velocity
        blobTouch.GetPhysics().ResetVelocity();
        motor.GetPhysics().ResetVelocity();

        blobTouch.GetPhysics().AddForce(impactForce * pushBackForce);
        motor.GetPhysics().AddForce(-impactVelocity * returnPushBackForce);

        // Set health
        blobTouch.GetHealth().OnDamageImpact(impactForce.sqrMagnitude);

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