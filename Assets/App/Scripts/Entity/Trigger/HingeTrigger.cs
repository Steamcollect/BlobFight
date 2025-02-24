using Unity.VisualScripting;
using UnityEngine;

public class HingeTrigger : CollisionTrigger
{
    //[Header("Settings")]

    [Header("References")]
    [SerializeField] HingeHealth health;

    [SerializeField] AnimationCurve damageBySpeedCurve;

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

    public void SetScript(HingeHealth hingeHealth, AnimationCurve damageCurve)
    {
        health = hingeHealth;
        damageBySpeedCurve = damageCurve;
    }

    void OnBlobCollision(BlobMotor blob)
    {
        Vector3 velocity = blob.joint.GetVelocity();
        float velocityMagnitude = velocity.sqrMagnitude;

        Vector3 direction = velocity.normalized;

        float directnessFactor = Mathf.Max(Mathf.Abs(direction.x), Mathf.Abs(direction.y));

        float damageMultiplier = Mathf.Lerp(0f, 1f, directnessFactor);

        int damage = (int)damageBySpeedCurve.Evaluate(velocityMagnitude * blob.joint.mass * damageMultiplier);
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