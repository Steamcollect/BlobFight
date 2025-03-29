using UnityEngine;

public class ThunderCollision : CollisionTrigger
{
    [Header("Settings")]
    [SerializeField] private float expulsionUpForce;
    [SerializeField] private float expulsionRightForce;
    [SerializeField] private float damage;

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
            motor.GetHealth().OnDamageImpact(damage);
            blobPhysics.AddForce(transform.up * expulsionUpForce);

            Vector3 horizontalForce = Vector3.zero;

            if (transform.position.x > collider.transform.position.x)
            {
                horizontalForce = -transform.right * expulsionRightForce;
            }
            else
            {
                horizontalForce = transform.right * expulsionRightForce;
            }

            blobPhysics.AddForce(horizontalForce);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EjectBlob(collision);
    }
}