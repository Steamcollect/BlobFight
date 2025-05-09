using DG.Tweening;
using System;
using UnityEngine;

public class BlobPhysics : MonoBehaviour, IPausable
{
    [Header("References")]
    [SerializeField] BlobMotor motor;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] CircleCollider2D collid;
    [SerializeField] Transform buttomPos;

    [Header("Input")]
    [SerializeField] RSE_SpawnBlob rseSpawnBlob;
    [SerializeField] RSE_OnGameStart rseOnGameStart;
    [SerializeField] RSE_OnFightStart rseOnFightStart;
    [SerializeField] RSE_OnFightEnd rseOnFightEnd;

    [HideInInspector] public Vector2 lastVelocity;
    private Action<Collision2D> onCollisionEnter;
    private Action<Collision2D> onCollisionStay;
    private Action<Collision2D> OnCollisionExit;
    public Action onJointsConnected;
    private float baseColliderRadius = 0;

    private void OnEnable()
    {
        rseOnGameStart.action += LockRB;
        rseOnFightStart.action += UnlockRB;
        rseOnFightEnd.action += LockRB;
    }

    private void OnDisable()
    {
        transform.DOKill();
        rseOnGameStart.action -= LockRB;
        rseOnFightStart.action -= UnlockRB;
        rseOnFightEnd.action -= LockRB;
    }

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

    public void LockRB()
    {
        rb.bodyType = RigidbodyType2D.Static;
    }

    public void UnlockRB()
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
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

    private void OnCollisionStay2D(Collision2D collision)
    {
        onCollisionStay?.Invoke(collision);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        OnCollisionExit?.Invoke(collision);
    }

    public void SyncColliderRadiusToVisual(Vector2 visualScale, Vector2 visualInitScale)
    {
        if (baseColliderRadius == 0)
        {
            baseColliderRadius = collid.radius;
        }

        float normalizedX = visualScale.x / visualInitScale.x;
        collid.radius = baseColliderRadius * normalizedX;
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

    public void Pause()
    {
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    public void Resume()
    {
        if(motor.IsAlive()) rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
}