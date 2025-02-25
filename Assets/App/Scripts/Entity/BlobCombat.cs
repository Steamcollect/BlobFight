using UnityEngine;
public class BlobCombat : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float pushBackForce;
    [SerializeField] float returnPushBackForce;

    [Header("References")]
    [SerializeField] BlobTrigger blobTrigger;
    [SerializeField] BlobJoint blobJoint;

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
        if (blob.GetJoint().GetVelocity().sqrMagnitude < velocity)
        {
            Vector2 direction = (blob.GetJoint().GetJointsCenter() - blobJoint.GetJointsCenter()).normalized;
            blob.GetJoint().AddForce(direction * pushBackForce * velocity);
            blobJoint.AddForce(-direction * returnPushBackForce * velocity);
        }
    }
}