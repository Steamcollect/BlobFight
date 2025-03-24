using UnityEngine;

public class HingeTrigger : CollisionTrigger
{
    //[Header("Settings")]

    [Header("References")]
    [SerializeField] HingeHealth health;

    [SerializeField] int maxRbVelocity;
    [SerializeField] AnimationCurve damageBySpeeCurve;

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

    public void SetScript(HingeHealth hingeHealth, int maxRbVelocity, AnimationCurve damageBySpeeCurve)
    {
        health = hingeHealth;
        this.maxRbVelocity = maxRbVelocity;
        this.damageBySpeeCurve = damageBySpeeCurve;
    }

    void OnBlobCollision(BlobMotor blob, Collision2D collision)
    {
        Vector3 velocity = blob.GetPhysics().GetVelocity();
        float velocityMagnitude = velocity.sqrMagnitude;

        int damage = (int)damageBySpeeCurve.Evaluate(velocityMagnitude / maxRbVelocity) * (int)blob.GetPhysics().GetComponent<Rigidbody2D>().mass;
        health.TakeDamage(damage);
    }

    void OnCollision(Collision2D collision)
    {
        if(collision.gameObject.TryGetComponent(out Rigidbody2D rb))
        {
            if(!collision.gameObject.GetComponent<CustomCollision>())
            {
                int damage = (int)damageBySpeeCurve.Evaluate(rb.velocity.sqrMagnitude / maxRbVelocity) * (int)rb.mass;
                health.TakeDamage(damage);
            }
            else
            {
                int damage = (int)(rb.velocity.sqrMagnitude * rb.mass);
                health.TakeDamage(damage);
            }
        }

        if (collision.gameObject.TryGetComponent(out Damagable damagable))
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