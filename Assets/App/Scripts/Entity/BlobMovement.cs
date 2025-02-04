using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Drawing;
using UnityEngine.Windows;

public class BlobMovement : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float idleDistanceMult;
    [SerializeField, Range(0, 1)] float idleDamping;
    [SerializeField] float idleFrequency;

    [Space(10)]
    [SerializeField] float openDistanceMult;
    [SerializeField, Range(0, 1)] float openDamping;
    [SerializeField] float openFrequency;

    [Space(10)]
    [SerializeField] float drag;
    [SerializeField] float angularDrag;
    [Space(5)]
    [SerializeField] float moveSpeed;
    [SerializeField] float pushForce;

    [Header("References")]
    [SerializeField] EntityInput entityInput;
    [SerializeField] BlobJoint blobJoint;

    class MySpringJoint
    {
        public SpringJoint2D spring;
        public float distance;

        public MySpringJoint(SpringJoint2D spring, float distance)
        {
            this.spring = spring;
            this.distance = distance;
        }
    }

    private void OnEnable()
    {
        blobJoint.onJointsConnected += SetupJoint;
    }
    private void OnDisable()
    {
        entityInput.GetInput("Jump").OnKeyDown -= OpenBlob;
        entityInput.GetInput("Jump").OnKeyUp -= CloseBlob;
        entityInput.GetInput("Move").OnUpdateFloat -= Move;

        blobJoint.onJointsConnected -= SetupJoint;
    }

    private void Start()
    {
        entityInput.GetInput("Jump").OnKeyDown += OpenBlob;
        entityInput.GetInput("Jump").OnKeyUp += CloseBlob;
        entityInput.GetInput("Move").OnUpdateFloat += Move;
    }

    private void FixedUpdate()
    {
        blobJoint.SetCollidersPosOffset(.5f);
    }

    void SetupJoint()
    {
        blobJoint.SetDrag(drag);
        blobJoint.SetAngularDrag(angularDrag);
        
        blobJoint.MultiplyInitialSpringDistance(idleDistanceMult);
        blobJoint.SetDamping(idleDamping);
        blobJoint.SetFrequency(idleFrequency);
    }

    void OpenBlob()
    {
        blobJoint.MultiplyInitialSpringDistance(openDistanceMult);
        blobJoint.SetDamping(openDamping);
        blobJoint.SetFrequency(openFrequency);
    }
    void CloseBlob()
    {
        blobJoint.MultiplyInitialSpringDistance(idleDistanceMult);
        blobJoint.SetDamping(idleDamping);
        blobJoint.SetFrequency(idleFrequency);
    }

    void Move(float input)
    {
        blobJoint.Move(Vector2.right * input * moveSpeed);
    }
}