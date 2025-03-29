using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float pushBackForce;
    [SerializeField] private float damage;
    [SerializeField] private AnimationCurve powerCurve;
    [SerializeField] private float range;

    private void Start()
    {
        Explode();
        Invoke("Explode", 1);
    }

    private void Explode()
    {
        Vector2 explosionPosition = transform.position;
        Collider2D[] hits = Physics2D.OverlapCircleAll(explosionPosition, range);

        foreach (Collider2D hit in hits)
        {
            Vector2 targetPosition = hit.transform.position;
            float distance = Vector2.Distance(targetPosition, explosionPosition);
            float power = powerCurve.Evaluate(distance / range);
            Vector2 forceDirection = (targetPosition - explosionPosition).normalized;

            if (hit.TryGetComponent(out EntityHealth health))
            {
                health.TakeDamage(Mathf.RoundToInt(damage * power));
            }

            if (hit.TryGetComponent(out Rigidbody2D rb))
            {
                rb.AddForce(forceDirection * pushBackForce * power, ForceMode2D.Impulse);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}