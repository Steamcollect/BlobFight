using System;
using UnityEngine;
public class BlobCombat : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float pushForce;
    [SerializeField] float pushForcePercentage;

    [Space(10)]
    [SerializeField] float pointRadius = .5f;

    [Header("References")]
    [SerializeField] Transform[] points;
    [SerializeField] LayerMask targetLayer;

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    //[Header("Input")]
    //[Header("Output")]

    private void FixedUpdate()
    {
        //Vector2 center = CalculateBlobCenter();
        //RaycastHit2D[] hits = Physics2D.CircleCastAll(center, Vector2.Distance(points[0].position, center), Vector2.up, targetLayer);

        //foreach (RaycastHit2D hit in hits )
        //{
        //    if(hit.transform.TryGetComponent(out BlobMovement blobMovement) && hit.transform != transform)
        //    {
        //        print("blob movement");
        //        blobMovement.PushBack((blobMovement.CalculateBlobCenter() - CalculateBlobCenter()) * pushForce * pushForcePercentage);
        //    }
        //}
    }

    private Vector2 CalculateBlobCenter()
    {
        Vector2 sum = Vector2.zero;
        foreach (Transform point in points)
        {
            sum += (Vector2)point.position;
        }
        return sum / points.Length;
    }
}