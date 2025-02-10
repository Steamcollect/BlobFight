using System.Collections.Generic;
using UnityEngine;

public class BlobVisual : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float outlineThickness = 0.1f;

    [Header("References")]
    [SerializeField] MeshFilter outlineMeshFilter;
    [SerializeField] MeshRenderer outlineRenderer;
    [SerializeField] MeshFilter fillMeshFilter;
    [SerializeField] MeshRenderer fillRenderer;

    [Space(10)]
    [SerializeField] BlobJoint blobJoint;

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

    public void Setup(BlobColor colors)
    {
        Material mat = new Material(outlineRenderer.material);
        mat.color = colors.outlineColor;
        outlineRenderer.material = mat;

        mat = new Material(fillRenderer.material);
        mat.color = colors.fillColor;
        fillRenderer.material = mat;
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
        foreach (Rigidbody2D point in blobJoint.jointsRb)
        {
            sum += (Vector2)point.transform.position;
        }
        return sum / blobJoint.jointsRb.Length;
    }

    private void UpdateOutlineMesh()
    {
        outlineMesh.Clear();

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        for (int i = 0; i < blobJoint.jointsRb.Length; i++)
        {
            vertices.Add(blobJoint.jointsRb[i].position);
            vertices.Add(
                (blobCenter
                + (Vector2)blobJoint.jointsRb[i].transform.localPosition
                + ((Vector2)blobJoint.jointsRb[i].transform.localPosition - blobCenter).normalized
                * outlineThickness) - blobCenter);
        }

        for (int i = 0; i < blobJoint.jointsRb.Length * 2; i += 2)
        {
            int next = (i + 2) % (blobJoint.jointsRb.Length * 2);
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

        Vector3[] pointsPos = new Vector3[blobJoint.jointsRb.Length];
        for (int i = 0; i < blobJoint.jointsRb.Length; i++)
        {
            pointsPos[i] = blobJoint.jointsRb[i].position;
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

    public void Show()
    {
        outlineMeshFilter.gameObject.SetActive(true);
        fillMeshFilter.gameObject.SetActive(true);
    }
    public void Hide()
    {
        outlineMeshFilter.gameObject.SetActive(false);
        fillMeshFilter.gameObject.SetActive(false);
    }
}