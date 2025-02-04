using System;
using System.Collections.Generic;
using UnityEngine;

public class BlobJoint : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float startingDistanceFromCenter;

    [Header("References")]
    public Rigidbody2D[] jointsRb;
    List<Joint> joints = new List<Joint>();

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    //[Header("Input")]
    //[Header("Output")]
    public Action onJointsConnected;

    private void Start()
    {
        SetupSprings();
    }
    void SetupSprings()
    {
        for (int i = 0; i < jointsRb.Length; i++)
        {
            Joint joint = jointsRb[i].gameObject.AddComponent<Joint>();
            joint.rb = jointsRb[i];
            jointsRb[i].freezeRotation = true;

            joint.collid = jointsRb[i].gameObject.GetComponent<Collider2D>();

            for (int j = i + 1; j < jointsRb.Length; j++)
            {
                SpringJoint2D spring = jointsRb[i].gameObject.AddComponent<SpringJoint2D>();
                spring.connectedBody = jointsRb[j];
                spring.autoConfigureDistance = false;

                float distance = Vector2.Distance(jointsRb[i].transform.position, jointsRb[j].transform.position);
                spring.distance = distance;

                joint.jointsConnected.Add(new Joint.Spring(distance, spring, jointsRb[j]));
            }

            joints.Add(joint);
        }

        onJointsConnected?.Invoke();
    }

    public void MoveJointsByTransform(Vector2 newCenterPosition)
    {
        Vector2 currentCenter = GetJointsCenter();
        Vector2 offset = newCenterPosition - currentCenter;
        foreach (var joint in jointsRb)
        {
            joint.transform.position += (Vector3)offset;
        }

        ResetJoints();
    }
    void ResetJoints()
    {
        foreach (var joint in joints)
        {
            joint.rb.velocity = Vector2.zero;
            joint.ResetSpringDistance();
        }
    }

    #region SpringJoint
    public void MultiplyInitialSpringDistance(float multiplier)
    {
        foreach (var joint in joints)
        {
            joint.MultiplyInitialSpringDistance(multiplier);
        }
    }
    public void ResetSpringDistance()
    {
        foreach (var joint in joints)
        {
            joint.ResetSpringDistance();
        }
    }

    public void SetDamping(float damping)
    {
        foreach (var joint in joints)
        {
            joint.SetDamping(damping);
        }
    }
    public void SetFrequency(float frequency)
    {
        foreach (var joint in joints)
        {
            joint.SetFrequency(frequency);
        }
    }
    #endregion

    #region Rigidbody
    public void Move(Vector2 force)
    {
        foreach (var joint in joints)
        {
            joint.Move(force);
        }
    }
    public void SetDrag(float drag)
    {
        foreach (var joint in joints)
        {
            joint.SetDrag(drag);
        }
    }
    public void SetAngularDrag(float angularDrag)
    {
        foreach (var joint in joints)
        {
            joint.SetAngularDrag(angularDrag);
        }
    }
    #endregion

    public void SetCollidersPosOffset(float dist)
    {
        Vector2 center = GetJointsCenter();

        foreach (var joint in joints)
        {
            joint.SetCollidPos(center, dist);
        }
    }

    public Vector2 GetJointsCenter()
    {
        Vector2 sum = Vector2.zero;
        foreach (Rigidbody2D joint in jointsRb)
        {
            sum += (Vector2)joint.transform.position;
        }
        return sum / jointsRb.Length;
    }

    private void OnValidate()
    {
        if (startingDistanceFromCenter <= .05f)
        {
            startingDistanceFromCenter = .05f;
            return;
        }
        AdjustPointsPosition();
    }
    private void AdjustPointsPosition()
    {
        if (jointsRb.Length == 0) return;
        float angleStep = 360f / jointsRb.Length;
        for (int i = 0; i < jointsRb.Length; i++)
        {
            // Set point position
            float angle = angleStep * i * Mathf.Deg2Rad;
            Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            jointsRb[i].transform.position = (Vector2)transform.position + dir * startingDistanceFromCenter;

            // Set collider pos offset
            Vector2 center = GetJointsCenter();
            CircleCollider2D collider = jointsRb[i].GetComponent<CircleCollider2D>();
            if (collider != null)
            {
                Vector2 directionToCenter = (center - (Vector2)jointsRb[i].position).normalized;
                collider.offset = directionToCenter * 0.5f;
            }
        }
    }
}