using UnityEngine;
using System.Collections.Generic;
using System;

public class BlobTrigger : CollisionTrigger
{
    [Header("Settings")]
    [SerializeField] string groundableTag;
    [SerializeField] bool isGrounded = false;

    [Header("References")]
    [SerializeField] BlobJoint blobJoint;

    List<GameObject> groundables = new();

    public Action OnGroundedEnter, OnGroundedExit;

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
            if (!isGrounded)
            {
                isGrounded = true;
                OnGroundedEnter?.Invoke();
            }

            groundables.Add(collision.gameObject);
        }
    }
    void OnExit(Collision2D collision)
    {
        groundables.Remove(collision.gameObject);
        if(groundables.Count <= 0)
        {
            OnGroundedExit?.Invoke();
            isGrounded = false;
        }
    }

    public bool IsGrounded() { return isGrounded; }
}