using System;
using UnityEngine;
using System.Linq;

public class CollisionTrigger : MonoBehaviour
{
    public Action<Collision2D> OnCollisionEnter, OnCollisionExit;
    public Action<BlobMotor> OnCollisionEnterWithBlob, OnCollisionExitWithBlob;
    public Action<BlobMotor, Collision2D> OnBlobCollisionEnter;
    public Action<ContactPoint2D> OnCollisionExitGetLastContact;

    public Action<Collider2D> OnTriggerEnter, OnTriggerExit;
    public Action<BlobMotor> OnTriggerEnterWithBlob, OnTriggerExitWithBlob;

    private SerializableDictionary<GameObject, ContactPoint2D> lastContacts = new();

    protected void OnEnterCollision(Collision2D collision)
    {
        if (collision.transform.TryGetComponent(out BlobPhysics blob))
        {
            OnCollisionEnterWithBlob?.Invoke(blob.GetMotor());
            OnBlobCollisionEnter?.Invoke(blob.GetMotor(), collision);
        }
        else
        {
            OnCollisionEnter?.Invoke(collision);
        }

        // Stocke le dernier point de contact et sa normale
        if (collision.contactCount > 0)
        {
            lastContacts[collision.gameObject] = collision.GetContact(collision.contactCount - 1);
        }
    }

    protected void OnStayCollision(Collision2D collision)
    {
        if (lastContacts.ContainsKey(collision.gameObject))
        {
            lastContacts[collision.gameObject] = collision.GetContact(collision.contactCount - 1);
        }
    }

    protected void OnExitCollision(Collision2D collision)
    {
        if (lastContacts.TryGetValue(collision.gameObject, out ContactPoint2D lastPoint))
        {
            OnCollisionExitGetLastContact?.Invoke(lastPoint);
        }

        // Check if blob
        if (collision.transform.CompareTag("Blob"))
        {
            OnCollisionExitWithBlob?.Invoke(collision.gameObject.GetComponent<BlobPhysics>().GetMotor());
        }
        else
        {
            OnCollisionExit?.Invoke(collision);
        }

        lastContacts.Remove(collision.gameObject);
    }

    protected void OnEnterTrigger(Collider2D collider)
    {
        // Check if blob
        if (collider.transform.TryGetComponent(out BlobPhysics blob))
        {
            OnTriggerEnterWithBlob?.Invoke(blob.GetMotor());
        }
        else
        {
            OnTriggerEnter?.Invoke(collider);
        }
    }

    protected void OnExitTrigger(Collider2D collider)
    {
        // Check if blob
        if (collider.transform.TryGetComponent(out BlobPhysics blob))
        {
            OnTriggerExitWithBlob?.Invoke(blob.GetMotor());
        }
        else
        {
            OnTriggerExit?.Invoke(collider);
        }
    }

    public ContactPoint2D[] GetAllContacts()
    {
        return lastContacts.Values.ToArray();
    }
}