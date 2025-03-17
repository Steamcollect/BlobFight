using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlobCombat : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float pushBackForce;
    [SerializeField] float returnPushBackForce;
    [SerializeField] float extendForceMultiplier;

    [Space(5)]
    [SerializeField] float parryMaxTime;
    [SerializeField] float paryForceMultiplier;

    bool canFight = true;

    [Header("References")]
    [SerializeField] BlobTrigger trigger;
    [SerializeField] BlobJoint joint;
    [SerializeField] BlobMovement movement;
    [SerializeField] BlobParticle particle;

    LayerMask currentLayer;

    List<BlobMotor> blobsTouch = new();

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    //[Header("Input")]
    //[Header("Output")]

    private void OnDisable()
    {
        trigger.OnBlobCollisionEnter -= OnBlobCollisionEnter;
    }

    private void Start()
    {
        Invoke("LateStart", .05f);
    }
    void LateStart()
    {
        trigger.OnBlobCollisionEnter += OnBlobCollisionEnter;
    }

    void OnBlobCollisionEnter(BlobMotor blobTouch, Collision2D collision)
    {
        if (blobsTouch.Contains(blobTouch) || !canFight) return;

        float speed = joint.GetVelocity().sqrMagnitude;
        float blobTouchSpeed = blobTouch.GetJoint().GetVelocity().sqrMagnitude;
        
        Vector2 propulsionDir = (blobTouch.GetJoint().GetJointsCenter() - joint.GetJointsCenter()).normalized;

        Vector2 impactVelocity = Vector2.zero;
        Vector2 impactForce = Vector2.zero;


        if (movement.IsExtend() && movement.GetExtendTime() < parryMaxTime && speed < blobTouchSpeed)
        {
            print("Parry");

            impactVelocity = blobTouch.GetJoint().GetVelocity() * blobTouchSpeed;
            impactForce = impactVelocity * blobTouch.GetHealth().GetPercentage() * paryForceMultiplier;

            blobTouch.GetJoint().ResetVelocity();
        }
        else if (!blobTouch.GetMovement().IsExtend() && movement.IsExtend())
        {
            impactVelocity = propulsionDir * speed;
            impactForce = impactVelocity * blobTouch.GetHealth().GetPercentage() * extendForceMultiplier;

            Debug.DrawLine(blobTouch.GetJoint().GetJointsCenter(), blobTouch.GetJoint().GetJointsCenter() + impactVelocity, Color.blue, 1);

            blobTouch.GetTrigger().ExludeLayer(currentLayer, .1f);
        }
        else if ((!movement.IsExtend() && !blobTouch.GetMovement().IsExtend()) || (movement.IsExtend() && blobTouch.GetMovement().IsExtend()))
        {
            if (speed < blobTouchSpeed) return; // Do not add force in this case

            impactVelocity = propulsionDir * speed;
            impactForce = impactVelocity * blobTouch.GetHealth().GetPercentage();
        }
        else return;

        // Set new velocity
        blobTouch.GetJoint().AddForce(impactForce * pushBackForce);
        joint.AddForce(-impactVelocity * returnPushBackForce);

        // Set health
        blobTouch.GetHealth().OnDamageImpact(impactForce.sqrMagnitude);

        particle.DoHitParticle(collision.GetContact(0).point, collision.GetContact(0).normal, impactForce.sqrMagnitude);

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