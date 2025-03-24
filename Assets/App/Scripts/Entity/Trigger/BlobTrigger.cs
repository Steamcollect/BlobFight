using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

public class BlobTrigger : CollisionTrigger
{
    [Header("Settings")]
    [SerializeField, TagName] string groundableTag;
    [SerializeField, TagName] string slidableTag;
    [SerializeField, TagName] string windTag;

    [Space(5)]
    [SerializeField] bool isGrounded = false;
    [SerializeField] bool isSliding = false;
    [SerializeField] bool isInWind = false;

    [Header("References")]
    [SerializeField] BlobPhysics physics;

    List<GameObject> groundables = new();
    List<GameObject> slidables = new();
    int windsTouchCount;

    public Action<Collision2D> OnGroundedEnter, OnGroundedExit;
    public Action OnGroundTouch;
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
        physics.RemoveOnCollisionEnterListener(OnEnterCollision);
        physics.RemoveOnCollisionExitListener(OnExitCollision);

        OnCollisionEnter -= OnEnter;
        OnCollisionExit -= OnExit;
    }

    private void Start()
    {
        Invoke("LateStart", .05f);
    }
    void LateStart()
    {
        physics.AddOnCollisionEnterListener(OnEnterCollision);
        physics.AddOnCollisionExitListener(OnExitCollision);

        OnCollisionEnter += OnEnter;
        OnCollisionExit += OnExit;
    }

    void OnEnter(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(groundableTag))
        {
            isGrounded = true;

            OnGroundedEnter?.Invoke(collision);
            OnGroundTouch?.Invoke();
            groundables.Add(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag(slidableTag))
        {
            isSliding = true;

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
            isSliding = false;
            OnSlidableExit?.Invoke(collision);
        }
    }

    public void OnWindEnter()
    {
        windsTouchCount++;
        isInWind = true;
    }
    public void OnWindExit()
    {
        windsTouchCount--;
        if (windsTouchCount < 0) windsTouchCount = 0;

        if(windsTouchCount <= 0)
        {
            isInWind = false;
        }
    }

    public bool IsGrounded() { return isGrounded; }
    public bool IsSliding() { return isSliding; }
    public bool IsInWind() { return isInWind; }

    public void ExludeLayer(LayerMask layerToExclude, float excludingTime)
    {
        LayerMask combine = this.layerToExclude | layerToExclude;
        this.layerToExclude = combine;
        physics.SetLayerToExlude(this.layerToExclude);

        StartCoroutine(RemoveExludeLayer(layerToExclude, excludingTime));
    }
    IEnumerator RemoveExludeLayer(LayerMask layerToExclude, float excludingTime)
    {
        yield return new WaitForSeconds(excludingTime);
        this.layerToExclude = this.layerToExclude & ~layerToExclude;
        physics.SetLayerToExlude(this.layerToExclude);
    }

    public void SetLayerToExclude(LayerMask layerToExclude)
    {
        this.layerToExclude = layerToExclude;
    }
}