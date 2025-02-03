using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class BlobMovement : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float startingDistanceFromCenter;

    [Space(10)]
    [SerializeField] float idleDistanceMult;
    [SerializeField, Range(0, 1)] float idleDampingRatio;
    [SerializeField] float idleFrequency;

    [Space(10)]
    [SerializeField] float openDistanceMult;
    [SerializeField, Range(0, 1)] float openDampingRatio;
    [SerializeField] float openFrequency;

    [Space(10)]
    [SerializeField] float drag;
    [SerializeField] float moveForce;
    [SerializeField] float pushForce;

    [Header("References")]
    [SerializeField] EntityInput entityInput;

    [Space(10)]
    [SerializeField] Rigidbody2D[] points;
    List<MySpringJoint> springs = new();

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

    [SerializeField] PolygonCollider2D polygonCollider;

    private void OnDisable()
    {
        entityInput.GetInput("Jump").OnKeyDown -= OpenBlob;
        entityInput.GetInput("Jump").OnKeyUp -= CloseBlob;
        entityInput.GetInput("Move").OnUpdateFloat -= Move;
    }

    private void Start()
    {
        entityInput.GetInput("Jump").OnKeyDown += OpenBlob;
        entityInput.GetInput("Jump").OnKeyUp += CloseBlob;
        entityInput.GetInput("Move").OnUpdateFloat += Move;

        SetupSprings();
    }

    void SetupSprings()
    {
        for (int i = 0; i < points.Length; i++)
        {
            points[i].drag = drag;

            for (int j = 0; j < points.Length; j++)
            {
                if (points[i] != points[j])
                {
                    if (points[i].TryGetComponent(out SpringJoint2D _spring)
                        && _spring.connectedBody == points[j])
                    {
                        continue;
                    }
                    if (points[j].TryGetComponent(out SpringJoint2D __spring)
                        && __spring.connectedBody == points[j])
                    {
                        continue;
                    }

                    SpringJoint2D spring = points[i].AddComponent<SpringJoint2D>();

                    spring.connectedBody = points[j];
                    spring.autoConfigureDistance = false;
                    spring.dampingRatio = idleDampingRatio;
                    spring.frequency = idleFrequency;

                    float distance = Vector2.Distance(points[i].position, points[j].position);
                    spring.distance = distance * idleDistanceMult;

                    springs.Add(new MySpringJoint(spring, distance));
                }
            }
        }
    }

    void OpenBlob()
    {
        for (int i = 0; i < springs.Count; i++)
        {
            springs[i].spring.distance = springs[i].distance * openDistanceMult;
            springs[i].spring.dampingRatio = openDampingRatio;
            springs[i].spring.frequency = openFrequency;
        }
    }
    void CloseBlob()
    {
        for (int i = 0; i < springs.Count; i++)
        {
            springs[i].spring.distance = springs[i].distance * idleDistanceMult;
            springs[i].spring.dampingRatio = idleDampingRatio;
            springs[i].spring.frequency = idleFrequency;
        }
    }

    void Move(float value)
    {
        for (int i = 0; i < points.Length; i++)
        {
            points[i].AddForce(Vector2.right * value * moveForce);
        }
    }

    private void FixedUpdate()
    {
        UpdatePolygonCollider();
    }

    private Vector2 CalculateCenter()
    {
        Vector2 sum = Vector2.zero;
        foreach (Rigidbody2D point in points)
        {
            sum += point.position;
        }
        return sum / points.Length;
    }
    private void UpdatePolygonCollider()
    {
        if (points.Length < 3) return; // Un polygone doit avoir au moins 3 points

        Vector2[] colliderPoints = new Vector2[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            colliderPoints[i] = points[i].transform.localPosition;
        }
        polygonCollider.points = colliderPoints;
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
        if (points.Length == 0) return;
        float angleStep = 360f / points.Length;
        for (int i = 0; i < points.Length; i++)
        {
            float angle = angleStep * i * Mathf.Deg2Rad;
            Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            points[i].transform.position = (Vector2)transform.position + dir * startingDistanceFromCenter;
        }
    }
}