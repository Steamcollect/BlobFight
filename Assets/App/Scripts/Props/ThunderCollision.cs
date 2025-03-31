using UnityEngine;

public class ThunderCollision : CollisionTrigger
{
    [Header("Settings")]
    [SerializeField] private float expulsionUpForce;
    [SerializeField] private float expulsionRightForce;
    [SerializeField] private int percentageDamage;

    private void OnEnable()
    {
        OnTriggerEnter += EjectBlob;
    }

    private void OnDisable()
    {
        OnTriggerExit -= EjectBlob;
    }

    private void EjectBlob(Collider2D collider)
    {
        if (collider.TryGetComponent(out BlobPhysics blobPhysics))
        {
            var motor = blobPhysics.GetMotor();
            motor.GetHealth().AddPercentage(percentageDamage);

            Vector3 expulsionForce = transform.up * expulsionUpForce;
            Vector3 horizontalForce = Vector3.zero;

            if (transform.position.x > collider.transform.position.x)
            {
                horizontalForce = -transform.right * expulsionRightForce;
            }
            else
            {
                horizontalForce = transform.right * expulsionRightForce;
            }

            expulsionForce += horizontalForce * motor.GetHealth().GetPercentage();            
            blobPhysics.AddForce(expulsionForce);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EjectBlob(collision);
    }
}