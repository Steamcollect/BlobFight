using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class Explosion : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float pushBackForce;
    [SerializeField] float damage;
    [SerializeField] AnimationCurve powerCurve;

    [Space(10)]
    [SerializeField] float range;

    //[Header("References")]

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    //[Header("Input")]
    //[Header("Output")]

    private void Start()
    {
        Explode();
        Invoke("Explode", 1);
    }

    public void Explode()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, range);
        List<BlobMotor> blobsTouch = new();

        foreach (Collider2D hit in hits)
        {
            float distance = Vector2.Distance(hit.transform.position, transform.position);
            float power = powerCurve.Evaluate(distance / range);

            Vector2 direction = (hit.transform.position - transform.position).normalized;
            
            if (hit.TryGetComponent(out EntityHealth health))
            {
                health.TakeDamage((int)(damage * power));
            }

            if (hit.TryGetComponent(out MyJoint joint))
            {
                if (!blobsTouch.Contains(joint.parentMotor))
                {
                    joint.parentMotor.joint.AddForce(direction * pushBackForce * power);
                    joint.parentMotor.health.TakeDamage((int)(damage * power));
                }
                blobsTouch.Add(joint.parentMotor);
            }

            else if (hit.TryGetComponent(out Rigidbody2D rb))
            {
                rb.AddForce(direction * pushBackForce * power);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}