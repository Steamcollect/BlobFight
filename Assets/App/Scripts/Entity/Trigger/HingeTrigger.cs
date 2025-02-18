using UnityEngine;

public class HingeTrigger : EntityTrigger
{
    //[Header("Settings")]

    [Header("References")]
    [SerializeField] HingeHealth health;

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    //[Header("Input")]
    //[Header("Output")]

    private void OnEnable()
    {
        OnCollisionEnter += OnCollision;
        OnCollisionEnterWithBlob += OnBlobCollision;
    }
    private void OnDisable()
    {
        OnCollisionEnter -= OnCollision;
        OnCollisionEnterWithBlob -= OnBlobCollision;
    }

    void OnBlobCollision(BlobMotor blob)
    {
        int damage = (int)blob.joint.GetVelocity().sqrMagnitude * blob.joint.mass;
        health.TakeDamage(damage);
    }
    void OnCollision(Collision2D collision)
    {
        if(collision.gameObject.TryGetComponent(out Rigidbody2D rb))
        {
            int damage = (int)(rb.velocity.sqrMagnitude * rb.mass);
            health.TakeDamage(damage);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnEnterCollision(collision);
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        OnExitCollision(collision);
    }
}