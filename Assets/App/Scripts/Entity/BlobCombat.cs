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
        Vector2 direction = (blob.GetJoint().GetJointsCenter() - blobJoint.GetJointsCenter()).normalized;

        if (!blob.GetMovement().IsExtend() && blobMovement.IsExtend())
        {
            blob.GetJoint().AddForce(direction * pushBackForce * velocity * extendForceMultiplier);
            blobJoint.AddForce(-direction * returnPushBackForce * velocity);
        }
        else if(!blob.GetMovement().IsExtend() && !blobMovement.IsExtend())
        {
            blob.GetJoint().AddForce(direction * pushBackForce * velocity);
            blobJoint.AddForce(-direction * returnPushBackForce * velocity);
        }
    }
}