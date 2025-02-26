using UnityEngine;
using System.Collections.Generic;
using System;
using NUnit.Framework;

public class BlobTrigger : CollisionTrigger
{
    [Header("Settings")]
    [SerializeField] string groundableTag;
    [SerializeField] bool isGrounded = false;

    [SerializeField] string slidableTag;

    [Header("References")]
    [SerializeField] BlobJoint blobJoint;

    List<GameObject> groundables = new();
    List<GameObject> slidables = new();

    public Action<Collision2D> OnGroundedEnter, OnGroundedExit;
    public Action<Collision2D> OnSlidableEnter, OnSlidableExit;

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

        OnCollisionEnter -= OnEnter;
        OnCollisionExit -= OnExit;
    }

    private void Start()
    {
        Invoke("LateStart", .05f);
    }
    void LateStart()
    {
        blobJoint.AddOnCollisionEnterListener(OnEnterCollision);
        blobJoint.AddOnCollisionExitListener(OnExitCollision);

        OnCollisionEnter += OnEnter;
        OnCollisionExit += OnExit;
    }

    void OnEnter(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(groundableTag))
        {
            isGrounded = true;

            OnGroundedEnter?.Invoke(collision);
            groundables.Add(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag(slidableTag))
        {
            OnSlidableEnter?.Invoke(collision);
            slidables.Add(collision.gameObject);
        }
    }
    void OnExit(Collision2D collision)
    {
        groundables.Remove(collision.gameObject);
        if(groundables.Count <= 0)
        {
            isGrounded = false;
            OnGroundedExit?.Invoke(collision);
        }

        slidables.Remove(collision.gameObject);
        if(slidables.Count <= 0)
        {
            OnSlidableExit?.Invoke(collision);
        }
    }

    public bool IsGrounded() { return isGrounded; }
}