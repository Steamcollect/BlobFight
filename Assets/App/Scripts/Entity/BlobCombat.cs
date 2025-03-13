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
    [SerializeField] float paryMaxTime;
    [SerializeField] float paryForceMultiplier;

    bool canFight = true;

    [Header("References")]
    [SerializeField] BlobTrigger blobTrigger;
    [SerializeField] BlobJoint blobJoint;
    [SerializeField] BlobMovement blobMovement;
    [SerializeField] BlobParticle blobParticle;

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
        blobTrigger.OnBlobCollisionEnter -= OnBlobCollisionEnter;
    }

    private void Start()
    {
        Invoke("LateStart", .05f);
    }
    void LateStart()
    {
        blobTrigger.OnBlobCollisionEnter += OnBlobCollisionEnter;
    }

    void OnBlobCollisionEnter(BlobMotor blob, Collision2D collision)
    {
        if (blobsTouch.Contains(blob) || !canFight) return;

        float velocity = blobJoint.GetVelocity().sqrMagnitude;
        float blobVelocity = blob.GetJoint().GetVelocity().sqrMagnitude;
        
        Vector2 impactDir = (blob.GetJoint().GetJointsCenter() - blobJoint.GetJointsCenter()).normalized;

        if(blobMovement.IsExtend() && blobMovement.GetExtendTime() < paryMaxTime)
        {
            float mySpeed = blobJoint.GetVelocity().sqrMagnitude;
            float impactSpeed = blob.GetJoint().GetVelocity().sqrMagnitude;

            if(mySpeed <= impactSpeed)
            {
                Vector2 propulsionDir = CalculateExpulsionDirection(blob.GetTrigger().GetCollisions(), impactDir);
                Vector2 impactForce = propulsionDir * pushBackForce * blobVelocity * extendForceMultiplier;
                blob.GetJoint().AddForce(impactForce);

                Vector2 impact = -impactDir * returnPushBackForce * velocity * blob.GetHealth().GetPercentage() * paryForceMultiplier;
                blobJoint.AddForce(impact);

                blob.GetHealth().OnDamageImpact(impactForce.sqrMagnitude);
                blob.GetTrigger().ExludeLayer(currentLayer, .1f);

                blobParticle.HitParticle(collision.GetContact(0).point, collision.GetContact(0).normal, impact.sqrMagnitude);

                StartCoroutine(ImpactCooldown(blob));
            }
        }
        else if (!blob.GetMovement().IsExtend() && blobMovement.IsExtend())
        {
            Vector2 propulsionDir = CalculateExpulsionDirection(blob.GetTrigger().GetCollisions(), impactDir);
            //Debug.DrawLine(blob.GetJoint().GetJointsCenter(), blob.GetJoint().GetJointsCenter() + propulsionDir, Color.blue, 1);

            Vector2 impactForce = propulsionDir * pushBackForce * velocity * extendForceMultiplier;
            blob.GetJoint().AddForce(impactForce);

            Vector2 impact = -impactDir * returnPushBackForce * velocity * blob.GetHealth().GetPercentage();
            blobJoint.AddForce(impact);

            blob.GetHealth().OnDamageImpact(impactForce.sqrMagnitude);
            blob.GetTrigger().ExludeLayer(currentLayer, .1f);

            blobParticle.HitParticle(collision.GetContact(0).point, collision.GetContact(0).normal, impact.sqrMagnitude);

            StartCoroutine(ImpactCooldown(blob));
            StartCoroutine(blob.GetCombat().CanFightCooldown());
        }
        else if((!blobMovement.IsExtend() && !blob.GetMovement().IsExtend()) || (blobMovement.IsExtend() && blob.GetMovement().IsExtend()))
        {
            if(velocity > blobVelocity)
            {
                Vector2 impactForce = impactDir * pushBackForce * velocity;
                Vector2 impact = impactForce * blob.GetHealth().GetPercentage();
                blob.GetJoint().AddForce(impact);
                blob.GetHealth().OnDamageImpact(impactForce.sqrMagnitude);

                blobJoint.AddForce(-impactDir * returnPushBackForce * velocity);

                blobParticle.HitParticle(collision.GetContact(0).point, collision.GetContact(0).normal, impact.sqrMagnitude);

                StartCoroutine(ImpactCooldown(blob));
                StartCoroutine(blob.GetCombat().CanFightCooldown());
            }
        }
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

    public static Vector2 CalculateExpulsionDirection(List<Collision2D> worldCollisions, Vector2 impactDirection)
    {
        if (worldCollisions == null || worldCollisions.Count == 0)
            return impactDirection.normalized;

        Vector2 totalNormal = Vector2.zero;

        foreach (var collision in worldCollisions)
        {
            foreach (var contact in collision.contacts)
            {
                totalNormal += contact.normal;
            }
        }
        totalNormal.Normalize();

        Vector2 blockedComponent = Vector2.Dot(impactDirection, totalNormal) * totalNormal;
        Vector2 freeComponent = impactDirection - blockedComponent;

        if (freeComponent.magnitude < 0.1f)
        {
            freeComponent = new Vector2(-totalNormal.y, totalNormal.x); // Rotation 90°
        }

        return freeComponent.normalized;
    }

    public void SetLayer(LayerMask layer)
    {
        currentLayer = layer;
    }
}