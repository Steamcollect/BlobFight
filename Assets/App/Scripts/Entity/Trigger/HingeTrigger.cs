using Unity.VisualScripting;
using UnityEngine;

public class HingeTrigger : CollisionTrigger
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
        int damage = (int)(blob.joint.GetVelocity().sqrMagnitude * blob.joint.mass);
        health.TakeDamage(damage);
    }
    void OnCollision(Collision2D collision)
    {
        if(collision.gameObject.TryGetComponent(out Rigidbody2D rb))
        {
            int damage = (int)(rb.velocity.sqrMagnitude * rb.mass);
            health.TakeDamage(damage);
        }
        else if (collision.gameObject.TryGetComponent(out Damagable damagable))
        {
            switch (damagable.GetDamageType())
            {
                case Damagable.DamageType.Damage:
                    health.TakeDamage(damagable.GetDamage());
                    break;

                case Damagable.DamageType.Kill:
                    health.Die();
                    break;

                case Damagable.DamageType.Destroy:
                    gameObject.SetActive(false);
                    break;
            }
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