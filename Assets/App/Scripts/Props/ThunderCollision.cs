using System.Collections;
using UnityEngine;
public class ThunderCollision : CollisionTrigger
{
    [Header("Settings")]
    [SerializeField] private float expulsionUpForce;
    [SerializeField] private float expulsionRightForce;

    //[Header("References")]

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    //[Header("Input")]
    //[Header("Output")]
    private void OnEnable()
    {
        OnTriggerEnter += EjectBlob;
    }
    private void OnDisable()
    {
        OnTriggerExit -= EjectBlob;
    }
    private void EjectBlob(Collider2D collider)
    {
        if(collider.TryGetComponent(out BlobPhysics blobPhysics))
        {
            blobPhysics.AddForce(transform.up * expulsionUpForce);
            if(transform.position.x > collider.transform.position.x)
            {
                blobPhysics.AddForce(-transform.right * expulsionRightForce);
            }
            else
            {
                blobPhysics.AddForce(transform.right * expulsionRightForce);
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        EjectBlob(collision);
    }
}