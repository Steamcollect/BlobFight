using System.Collections.Generic;
using UnityEngine;
public class WindZone : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float windForce;
    [SerializeField] float directionAngle;
    Vector2 direction;

    //[Header("References")]
    List<Rigidbody2D> rbs = new();

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    //[Header("Input")]
    //[Header("Output")]

    private void FixedUpdate()
    {
        if(rbs.Count > 0)
        {
            foreach(var rbs in rbs)
            {
                rbs.AddForce(direction * windForce);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out Rigidbody2D rb))
        {
            rbs.Add(rb);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Rigidbody2D rb))
        {
            rbs.Remove(rb);
        }
    }

    private void OnValidate()
    {
        direction = new Vector2(Mathf.Sin(directionAngle * Mathf.Deg2Rad), Mathf.Cos(directionAngle * Mathf.Deg2Rad));
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + direction);
    }
}