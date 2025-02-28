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
        OnBlobCollisionEnter += OnBlobCollision;
    }
    private void OnDisable()
    {
        OnCollisionEnter -= OnCollision;
        OnBlobCollisionEnter -= OnBlobCollision;
    }

    public void SetScript(HingeHealth hingeHealth, AnimationCurve damageCurve)
    {
        health = hingeHealth;
        damageBySpeedCurve = damageCurve;
    }

    void OnBlobCollision(BlobMotor blob, Collision2D collision)
    {
        Vector3 velocity = blob.GetJoint().GetVelocity();
        float velocityMagnitude = velocity.sqrMagnitude;

        Vector2 contactNormal = collision.GetContact(0).normal;

        float impactFactor = Mathf.Abs(Vector2.Dot(velocity.normalized, contactNormal));

        float damageMultiplier = Mathf.Lerp(0f, 1f, impactFactor);

        int baseDamage = (int)damageBySpeedCurve.Evaluate(velocityMagnitude * blob.GetJoint().mass);

        int finalDamage = Mathf.RoundToInt(baseDamage * damageMultiplier);

        health.TakeDamage(finalDamage);
    }

    void OnCollision(Collision2D collision)
    {
        if(collision.gameObject.TryGetComponent(out Rigidbody2D rb))
        {
            if(!collision.gameObject.GetComponent<BallHamerCollision>())
            {
                int damage = (int)damageBySpeedCurve.Evaluate(rb.velocity.sqrMagnitude * rb.mass);
                health.TakeDamage(damage);
            }
            else
            {
                int damage = (int)(rb.velocity.sqrMagnitude * rb.mass);
                health.TakeDamage(damage);
            }
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