using System.Collections.Generic;
using UnityEngine;
public class BlobCombat : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float pushBackForce;
    [SerializeField] float returnPushBackForce;
    [SerializeField] float extendForceMultiplier;

    [Header("References")]
    [SerializeField] BlobTrigger blobTrigger;
    [SerializeField] BlobJoint blobJoint;
    [SerializeField] BlobMovement blobMovement;

    LayerMask currentLayer;

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    //[Header("Input")]
    //[Header("Output")]

    private void OnDisable()
    {
        blobTrigger.OnCollisionEnterWithBlob -= OnBlobCollisionEnter;
    }

    private void Start()
    {
        Invoke("LateStart", .05f);
    }
    void LateStart()
    {
        blobTrigger.OnCollisionEnterWithBlob += OnBlobCollisionEnter;
    }

    void OnBlobCollisionEnter(BlobMotor blob)
    {
        float velocity = blobJoint.GetVelocity().sqrMagnitude;
        Vector2 impactDir = (blob.GetJoint().GetJointsCenter() - blobJoint.GetJointsCenter()).normalized;

        if (!blob.GetMovement().IsExtend() && blobMovement.IsExtend())
        {
            Vector2 propulsionDir = CalculateExpulsionDirection(blob.GetTrigger().GetCollisions(), impactDir);

            blob.GetJoint().AddForce(propulsionDir * pushBackForce * velocity * extendForceMultiplier);
            blobJoint.AddForce(-impactDir * returnPushBackForce * velocity);
            blob.GetTrigger().ExludeLayer(currentLayer, .5f);
        }
        else if(!blob.GetMovement().IsExtend() && !blobMovement.IsExtend())
        {
            blob.GetJoint().AddForce(impactDir * pushBackForce * velocity);
            blobJoint.AddForce(-impactDir * returnPushBackForce * velocity);
        }
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