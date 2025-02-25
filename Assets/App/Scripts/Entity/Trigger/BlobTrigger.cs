using System;
using System.Collections.Generic;
using UnityEngine;

public class BlobTrigger : CollisionTrigger
{
    [Header("Settings")]
    [SerializeField] float shrinkRange;
    [SerializeField] float extendRange;

    [Space(5)]
    [SerializeField] LayerMask groundableLayer;

    [Header("References")]
    [SerializeField] BlobJoint blobJoint;

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    //[Header("Input")]
    //[Header("Output")]

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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, shrinkRange);
        Gizmos.DrawWireSphere(transform.position, extendRange);
    }
}