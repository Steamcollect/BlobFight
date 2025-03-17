using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

public class BlobTrigger : CollisionTrigger
{
    [Header("Settings")]
    [SerializeField, TagName] string groundableTag;
    [SerializeField] bool isGrounded = false;
    [SerializeField] bool isSliding = false;

    [SerializeField, TagName] string slidableTag;

    [Header("References")]
    [SerializeField] BlobJoint blobJoint;

    List<Collision2D> worldCollisions = new();
    List<GameObject> groundables = new();
    List<GameObject> slidables = new();

    public Action<Collision2D> OnGroundedEnter, OnGroundedExit;
    public Action<Collision2D> OnSlidableEnter, OnSlidableExit;

    LayerMask layerToExclude;

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
            isSliding = true;

            OnSlidableEnter?.Invoke(collision);
            slidables.Add(collision.gameObject);
        }

        print(collision.gameObject.name);
        worldCollisions.Add(collision);
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
            isSliding = false;
            OnSlidableExit?.Invoke(collision);
        }

        worldCollisions.Remove(collision);
    }
    public bool IsGrounded() { return isGrounded; }
    public bool IsSliding() { return isSliding; }
    public List<Collision2D> GetCollisions() { return worldCollisions; }

    public void ExludeLayer(LayerMask layerToExclude, float excludingTime)
    {
        LayerMask combine = this.layerToExclude | layerToExclude;
        this.layerToExclude = combine;
        blobJoint.SetLayerToExlude(this.layerToExclude);

        StartCoroutine(RemoveExludeLayer(layerToExclude, excludingTime));
    }
    IEnumerator RemoveExludeLayer(LayerMask layerToExclude, float excludingTime)
    {
        yield return new WaitForSeconds(excludingTime);
        this.layerToExclude = this.layerToExclude & ~layerToExclude;
        blobJoint.SetLayerToExlude(this.layerToExclude);
    }

    public void SetLayerToExclude(LayerMask layerToExclude)
    {
        this.layerToExclude = layerToExclude;
    }
}