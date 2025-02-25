using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MyJoint : MonoBehaviour, IPausable
{
    public BlobMotor parentMotor;
    public Rigidbody2D rb;
    public Collider2D collid;

    public List<Spring> jointsConnected = new();

    public Action<Collision2D> OnCollisionEnter, OnCollisionExit;

    #region SpringJoint
    public void MultiplyInitialSpringDistance(float multiplier)
    {
        foreach (var joint in jointsConnected)
        {
            joint.spring.distance = joint.distance * multiplier;
        }
    }
    public void ResetSpringDistance()
    {
        foreach (var joint in jointsConnected)
        {
            joint.spring.distance = joint.distance;
        }
    }
    public void SetDamping(float damping)
    {
        foreach (var joint in jointsConnected)
        {
            joint.spring.dampingRatio = damping;
        }
    }
    public void SetFrequency(float frequency)
    {
        foreach (var joint in jointsConnected)
        {
            joint.spring.frequency = frequency;
        }
    }
    #endregion

    #region Rigidbody
    Vector3 velocity;
    float angularVelocity;
    RigidbodyType2D bodyType;

    public void ResetVelocity()
    {
        rb.velocity = Vector3.zero;
    }
    public void SetVelocity(Vector2 velocity)
    {
        rb.velocity = velocity;
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
    #endregion

    #region Collision
    public void AddOnCollisionEnterListener(Action<Collision2D> action)
    {
        OnCollisionEnter += action;
    }
    public void RemoveOnCollisionEnterListener(Action<Collision2D> action)
    {
        OnCollisionEnter -= action;
    }
    public void AddOnCollisionExitListener(Action<Collision2D> action)
    {
        OnCollisionExit += action;
    }
    public void RemoveOnCollisionExitListener(Action<Collision2D> action)
    {
        OnCollisionExit -= action;
    }
    
    public void SetCollidPos(Vector2 center, float distance)
    {
        Vector2 dir = (center - (Vector2)rb.transform.localPosition).normalized;
        collid.offset = dir * distance;
    }
    #endregion

    public void Pause()
    {
        velocity = rb.velocity;
        angularVelocity = rb.angularVelocity;
        bodyType = rb.bodyType;

        rb.bodyType = RigidbodyType2D.Static;
    }
    public void Resume()
    {
        rb.bodyType = bodyType;
        rb.velocity = velocity;
        rb.angularVelocity = angularVelocity;
    }

    [System.Serializable]
    public class Spring
    {
        public float distance;
        public SpringJoint2D spring;
        public Rigidbody2D rbConnect;

        public Spring(float distance, SpringJoint2D spring, Rigidbody2D rbConnect)
        {
            this.distance = distance;
            this.spring = spring;
            this.rbConnect = rbConnect;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //print(collision.gameObject.name);
        OnCollisionEnter?.Invoke(collision);
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        OnCollisionExit?.Invoke(collision);
    }

    private void OnDrawGizmosSelected()
    {
        //for (int i = 0; i < jointsConnected.Count; i++)
        //{
        //    Gizmos.DrawLine(rb.transform.position, jointsConnected[i].rbConnect.transform.position);
        //}
    }
}