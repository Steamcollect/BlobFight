using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

public class BlobTrigger : CollisionTrigger
{
    [Header("Settings")]
    [SerializeField] private LayerMask groundableLayer;
    [SerializeField] private LayerMask slidableLayer;
    [SerializeField, TagName] private string windTag;
    [SerializeField] private bool isGrounded = false;
    [SerializeField] private bool isSliding = false;
    [SerializeField] private bool isInWind = false;
    [Space(10)]
    [SerializeField, TagName] string grassTag;
    [SerializeField, TagName] string stoneTag;
    [SerializeField, TagName] string metalTag;

    [Header("References")]
    [SerializeField] private BlobPhysics physics;
    [SerializeField] private BlobAudio blobAudio;
    public Action<Collision2D> OnGroundedEnter, OnGroundedExit;
    public Action OnGroundTouch;
    public Action<Collision2D> OnSlidableEnter, OnSlidableExit;
    public Action _OnWindEnter;

    private List<GameObject> groundables = new();
    private List<GameObject> slidables = new();
    private int windsTouchCount = 0;
    private LayerMask layerToExclude;

    public Action resetParent;
    public Action<Transform> setParent;

    [HideInInspector]public bool lockInteraction = false;

    private void OnDisable()
    {
        physics.RemoveOnCollisionEnterListener(OnEnterCollision);
        physics.RemoveOnCollisionExitListener(OnExitCollision);
        physics.RemoveOnCollisionStayListener(OnStayCollision);

        OnCollisionEnter -= OnEnter;
        OnCollisionExit -= OnExit;
    }

    private void Start()
    {
        Invoke(nameof(LateStart), 0.1f);
    }

    private void LateStart()
    {
        physics.AddOnCollisionEnterListener(OnEnterCollision);
        physics.AddOnCollisionExitListener(OnExitCollision);
        physics.AddOnCollisionStayListener(OnStayCollision);

        OnCollisionEnter += OnEnter;
        OnCollisionExit += OnExit;
    }

    private void OnEnter(Collision2D collision)
    {
        if(lockInteraction) return;

        if (((1 << gameObject.layer) & groundableLayer.value) == 0)
        {
            isGrounded = true;

            OnGroundedEnter?.Invoke(collision);
            OnGroundTouch?.Invoke();
            groundables.Add(collision.gameObject);
            setParent?.Invoke(collision.transform);
        }
        else if (((1 << gameObject.layer) & slidableLayer.value) == 0)
        {
            isSliding = true;

            OnSlidableEnter?.Invoke(collision);
            slidables.Add(collision.gameObject);
            setParent?.Invoke(collision.transform);
        }
        OnMapTouch(collision);
    }
    void OnMapTouch(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(grassTag))
        {
            blobAudio.PlayTouchGrassClip();
        }
        else if (collision.gameObject.CompareTag(stoneTag))
        {
            blobAudio.PlayTouchStoneClip();
        }
        else if (collision.gameObject.CompareTag(metalTag))
        {
            blobAudio.PlayTouchMetalClip();
        }
    }

    private void OnExit(Collision2D collision)
    {
        if (lockInteraction) return;

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

        if (!isGrounded && !isSliding)
        {
            resetParent?.Invoke();
        }
    }

    public void OnWindEnter()
    {
        windsTouchCount++;
        isInWind = true;

        _OnWindEnter.Invoke();
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

    private IEnumerator RemoveExludeLayer(LayerMask layerToExclude, float excludingTime)
    {
        yield return new WaitForSeconds(excludingTime);
        this.layerToExclude = this.layerToExclude & ~layerToExclude;
        physics.SetLayerToExlude(this.layerToExclude);
    }

    public void SetLayerToExclude(LayerMask layerToExclude)
    {
        this.layerToExclude = layerToExclude;
    }

    public void ResetTouchs()
    {
        groundables.Clear();
        slidables.Clear();
        isGrounded = false;
        isSliding = false;
        isInWind = false;
        windsTouchCount = 0;
    }
}