using System;
using System.Collections.Generic;
using UnityEngine;

public class BlobTrigger : MonoBehaviour
{
    //[Header("Settings")]

    [Header("References")]
    [SerializeField] BlobJoint blobJoint;
    
    List<GameObject> collisions = new();

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    //[Header("Input")]
    //[Header("Output")]
    public Action<Collision2D> OnCollisionEnter, OnCollisionExit;

    private void OnDisable()
    {
        blobJoint.RemoveOnCollisionEnterListener(OnEnterCollision);
        blobJoint.RemoveOnCollisionExitListener(OnExitCollision);
    }

    private void Start()
    {
        Invoke("LateStart", .05f);
    }
    void LateStart()
    {
        blobJoint.AddOnCollisionEnterListener(OnEnterCollision);
        blobJoint.AddOnCollisionExitListener(OnExitCollision);
    }

    void OnEnterCollision(Collision2D collision)
    {
        if (!collisions.Contains(collision.gameObject))
        {
            OnCollisionEnter?.Invoke(collision);
        }

        collisions.Add(collision.gameObject);
    }

    void OnExitCollision(Collision2D collision)
    {
        collisions.Remove(collision.gameObject);

        if (!collisions.Contains(collision.gameObject))
        {
            OnCollisionExit?.Invoke(collision);
        }
    }
}