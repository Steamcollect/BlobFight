using System.Collections.Generic;
using System;
using UnityEngine;

public class CollisionTrigger : MonoBehaviour
{
    //[Header("Settings")]

    //[Header("References")]
    protected List<GameObject> collisions = new();
    List<BlobMotor> blobs = new();

    Dictionary<GameObject, ContactPoint2D> lastContacts = new();

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    //[Header("Input")]
    //[Header("Output")]
    public Action<Collision2D> OnCollisionEnter, OnCollisionExit;
    public Action<BlobMotor> OnCollisionEnterWithBlob, OnCollisionExitWithBlob;
    public Action<BlobMotor, Collision2D> OnBlobCollisionEnter;
    public Action<ContactPoint2D> OnCollisionExitGetLastContact;

    public Action<Collider2D> OnTriggerEnter, OnTriggerExit;
    public Action<BlobMotor> OnTriggerEnterWithBlob, OnTriggerExitWithBlob;

    protected void OnEnterCollision(Collision2D collision)
    {
        if (!collisions.Contains(collision.gameObject))
        {
            // Check if blob
            if (collision.transform.TryGetComponent(out MyJoint joint))
            {
                if (!blobs.Contains(joint.parentMotor))
                {
                    OnCollisionEnterWithBlob?.Invoke(joint.parentMotor);
                    OnBlobCollisionEnter?.Invoke(joint.parentMotor, collision);
                }

                blobs.Add(joint.parentMotor);
            }
            else
            {
                OnCollisionEnter?.Invoke(collision);
            }
        }
        collisions.Add(collision.gameObject);

        // Stocke le dernier point de contact et sa normale
        if (collision.contactCount > 0)
        {
            lastContacts[collision.gameObject] = collision.GetContact(collision.contactCount - 1);
        }
    }
    protected void OnExitCollision(Collision2D collision)
    {
        collisions.Remove(collision.gameObject);

        if (!collisions.Contains(collision.gameObject))
        {
            if (lastContacts.TryGetValue(collision.gameObject, out ContactPoint2D lastPoint))
            {
                OnCollisionExitGetLastContact?.Invoke(lastPoint);
            }

            // Check if blob
            if (collision.transform.TryGetComponent(out MyJoint joint))
            {
                blobs.Remove(joint.parentMotor);

                if (!blobs.Contains(joint.parentMotor))
                {
                    OnCollisionExitWithBlob?.Invoke(joint.parentMotor);
                }
            }
            else
            {
                OnCollisionExit?.Invoke(collision);
            }

            lastContacts.Remove(collision.gameObject);
        }
    }

    protected void OnEnterTrigger(Collider2D collider)
    {
        if (!collisions.Contains(collider.gameObject))
        {
            // Check if blob
            if (collider.transform.TryGetComponent(out MyJoint joint))
            {
                if (!blobs.Contains(joint.parentMotor))
                {
                    OnTriggerEnterWithBlob?.Invoke(joint.parentMotor);
                }

                blobs.Add(joint.parentMotor);
            }
            else
            {
                OnTriggerEnter?.Invoke(collider);
            }
        }
        collisions.Add(collider.gameObject);
    }
    protected void OnExitTrigger(Collider2D collider)
    {
        collisions.Remove(collider.gameObject);

        if (!collisions.Contains(collider.gameObject))
        {
            // Check if blob
            if (collider.transform.TryGetComponent(out MyJoint joint))
            {
                blobs.Remove(joint.parentMotor);

                if (!blobs.Contains(joint.parentMotor))
                {
                    OnTriggerExitWithBlob?.Invoke(joint.parentMotor);
                }
            }
            else
            {
                OnTriggerExit?.Invoke(collider);
            }
        }
    }
}