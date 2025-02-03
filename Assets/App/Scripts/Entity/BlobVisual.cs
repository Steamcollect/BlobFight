using System.Collections.Generic;
using UnityEngine;

public class BlobVisual : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float pointRadius = 0.5f;
    [SerializeField] float outlineThickness = 0.1f;

    [Header("References")]
    [SerializeField] Transform[] points;
    [SerializeField] MeshFilter outlineMeshFilter;

    private Mesh outlineMesh;
    private Vector2 blobCenter;

    private void Awake()
    {
        outlineMesh = new Mesh();
        outlineMeshFilter.mesh = outlineMesh;
    }

    private void Update()
    {
        blobCenter = CalculateBlobCenter();
        UpdateOutlineMesh();
    }

    private Vector2 CalculateBlobCenter()
    {
        Vector2 sum = Vector2.zero;
        foreach (Transform point in points)
        {
            sum += (Vector2)point.position;
        }
        return sum / points.Length;
    }

    private void UpdateOutlineMesh()
    {
        outlineMesh.Clear();

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        for (int i = 0; i < points.Length; i++)
        {
            Vector2 point = (Vector2)points[i].localPosition;
            Vector2 dir = (blobCenter + point).normalized * (pointRadius + outlineThickness);

            vertices.Add(point);
            vertices.Add(point + dir);
        }

        for (int i = 0; i < points.Length * 2; i += 2)
        {
            int next = (i + 2) % (points.Length * 2);
            triangles.Add(i);
            triangles.Add(next);
            triangles.Add(i + 1);

            triangles.Add(i + 1);
            triangles.Add(next);
            triangles.Add(next + 1);
        }

        outlineMesh.vertices = vertices.ToArray();
        outlineMesh.triangles = triangles.ToArray();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(CalculateBlobCenter(), .1f);
    }
}