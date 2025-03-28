using DG.Tweening;
using System;
using UnityEngine;

public class BlobPhysics : MonoBehaviour, IPausable
{
    float baseColliderRadius = 0;

    [Header("References")]
    [SerializeField] BlobMotor motor;

    [SerializeField] Rigidbody2D rb;
    [SerializeField] CircleCollider2D collid;

    [Space(5)]
    [SerializeField] Transform buttomPos;

    // RSO
    // RSF
    // RSP

    //[Header("Input")]
    //[Header("Output")]

    [HideInInspector] public Vector2 lastVelocity;

    Action<Collision2D> onCollisionEnter, onCollisionStay, OnCollisionExit;
    public Action onJointsConnected;

    public void SetupLayer(LayerMask layerMask)
    {
        gameObject.layer = Mathf.RoundToInt(Mathf.Log(layerMask.value, 2)); // Convert layer to binary value
        collid.excludeLayers = layerMask;
    }

    public void Enable()
    {
        collid.enabled = true;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
    public void Disable()
    {
        collid.enabled = false;
        rb.constraints = RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;
    }

    private void FixedUpdate()
    {
        lastVelocity = rb.velocity;    
    }

    #region Joint
    public void MoveTo(Vector2 newCenterPosition)
    {
        transform.position = newCenterPosition;
    }
    public Vector2 GetCenter()
    {
        return transform.position;
    }

    #endregion

    #region Rigidbody
    public void ResetVelocity()
    {
        rb.velocity = Vector2.zero;
    }
    public void AddForce(Vector2 force)
    {
        rb.AddForce(force);

    }
    public void SetDrag(float drag)
    {
        rb.drag = drag;
    }

    public void SetGravity(float gravity)
    {
        rb.gravityScale = gravity;

    }
    public void SetMass(float mass)
    {
        rb.mass = mass;
    }

    public void SetVelocity(Vector2 velocity)
    {
        rb.velocity = velocity;
    }
    public Vector2 GetVelocity()
    {
        return rb.velocity;
    }
    public Rigidbody2D GetRigidbody() { return rb; }
    #endregion

    #region Collider
    #region CollisionEnter
    public void AddOnCollisionEnterListener(Action<Collision2D> action)
    {
        onCollisionEnter += action;
    }
    public void RemoveOnCollisionEnterListener(Action<Collision2D> action)
    {
        onCollisionEnter -= action;
    }
    #endregion
    #region CollisionExit
    public void AddOnCollisionExitListener(Action<Collision2D> action)
    {
        OnCollisionExit += action;
    }
    public void RemoveOnCollisionExitListener(Action<Collision2D> action)
    {
        OnCollisionExit -= action;
    }
    #endregion
    #region CollisionStay
    public void AddOnCollisionStayListener(Action<Collision2D> action)
    {
        onCollisionStay += action;
    }
    public void RemoveOnCollisionStayListener(Action<Collision2D> action)
    {
        onCollisionStay -= action;
    }
    #endregion

    public void SetLayerToExlude(LayerMask layerToExclude)
    {
        collid.excludeLayers = layerToExclude;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        onCollisionEnter?.Invoke(collision);
    }
    void OnCollisionStay2D(Collision2D collision)
    {
        onCollisionStay?.Invoke(collision);
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        OnCollisionExit?.Invoke(collision);
    }

    public void SyncColliderRadiusToVisual(Vector2 visualScale, Vector2 visualInitScale)
    {
        Vector2 normalizedScale = new Vector2(
            visualScale.x / visualInitScale.x,
            visualScale.y / visualInitScale.y
        );

        float smallestAxis = Mathf.Min(normalizedScale.x, normalizedScale.y);
        collid.radius = baseColliderRadius * smallestAxis;
    }

    public Vector2 GetButtomPosition()
    {
        return buttomPos.position;
    }
    #endregion

    public void ChangeScaleTarget(float targetScale, float time)
    {
        transform.DOKill();
        transform.DOScale(targetScale, time);
    }

    public BlobMotor GetMotor() { return motor; }

    private void OnDisable()
    {
        transform.DOKill();
    }

    public void Pause()
    {
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    public void Resume()
    {
        if(motor.IsAlive()) rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
}