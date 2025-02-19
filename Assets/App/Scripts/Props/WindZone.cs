using System.Collections.Generic;
using UnityEngine;
public class WindZone : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float windForce;

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
                rbs.AddForce(transform.up * windForce);
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
}