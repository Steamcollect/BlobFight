using System.Collections.Generic;
using UnityEngine;

public class BlobVisual : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float outlineThickness = 0.1f;

    [Header("References")]
    [SerializeField] Transform[] points;
    [SerializeField] MeshFilter outlineMeshFilter;
    [SerializeField] MeshFilter fillMeshFilter;

    private Mesh outlineMesh;
    private Mesh fillMesh;
    private Vector2 blobCenter;

    private void Awake()
    {
        outlineMesh = new Mesh();
        outlineMeshFilter.mesh = outlineMesh;

        fillMesh = new Mesh();
        fillMeshFilter.mesh = fillMesh;
    }

    private void Update()
    {
        blobCenter = CalculateBlobCenter();
        UpdateOutlineMesh();
        UpdateFillMesh();
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
            vertices.Add(points[i].position);
            vertices.Add((blobCenter + (Vector2)points[i].localPosition + ((Vector2)points[i].localPosition - blobCenter).normalized * outlineThickness) - blobCenter);
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

    private void UpdateFillMesh()
    {
        fillMesh.Clear();

        Vector3[] pointsPos = new Vector3[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            pointsPos[i] = points[i].position;
        }

        fillMesh.vertices = pointsPos;
        fillMesh.triangles = DrawFilledTriangles(pointsPos);
    }

    int[] DrawFilledTriangles(Vector3[] pointsPos)
    {
        int triangleAmout = pointsPos.Length - 2;
        List<int> newTriangles = new List<int>();
        for (int i = 0; i < triangleAmout; i++)
        {
            newTriangles.Add(0);
            newTriangles.Add(i+2);
            newTriangles.Add(i+1);
        }

        return newTriangles.ToArray();
    }
}