using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlobVisual : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] int shrinkBevelCount;
    [SerializeField, Range(0, .3f)] float shrinkHeightFactor;
    [Space(5)]
    [SerializeField] int extendBevelCount;
    [SerializeField, Range(0, .3f)] float extendHeightFactor;

    [Space(5)]
    [SerializeField] float outlineThickness = 0.1f;
    int bevelCount;
    float heightFactor;

    [Header("References")]
    [SerializeField] MeshFilter outlineMeshFilter;
    [SerializeField] MeshRenderer outlineRenderer;
    [SerializeField] MeshFilter fillMeshFilter;
    [SerializeField] MeshRenderer fillRenderer;

    [Space(10)]
    [SerializeField] BlobJoint blobJoint;

    private Mesh outlineMesh;
    private Mesh fillMesh;

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

    private void Start()
    {
        SetToShrink();
    }

    private void Update()
    {
        UpdateFillMesh();
        UpdateOutlineMesh();
    }

    private void UpdateFillMesh()
    {
        fillMesh.Clear();

        List<Vector3> pointsPos = new List<Vector3>();
        for (int i = 0; i < blobJoint.jointsRb.Length; i++)
        {
            Vector3 p0 = blobJoint.jointsRb[i].position;
            Vector3 p1 = blobJoint.jointsRb[(i + 1) % blobJoint.jointsRb.Length].position;

            // Calcul de la normale entre les deux points
            Vector3 segmentDirection = (p1 - p0).normalized;
            Vector3 normal = Vector3.Cross(segmentDirection, Vector3.forward).normalized * heightFactor;
            Vector3 midPoint = (p0 + p1) / 2 + normal;

            // Ajout des points intermédiaires basés sur une courbe de Bézier quadratique
            for (int j = 0; j <= bevelCount; j++)
            {
                float t = j / (float)(bevelCount + 1);
                Vector3 bezierPoint = Mathf.Pow(1 - t, 2) * p0 + 2 * (1 - t) * t * midPoint + Mathf.Pow(t, 2) * p1;
                pointsPos.Add(bezierPoint);
            }
        }

        fillMesh.vertices = pointsPos.ToArray();
        fillMesh.triangles = GetTriangles(pointsPos);
    }
    private void UpdateOutlineMesh()
    {
        outlineMesh.Clear();

        List<Vector3> outlinePoints = new List<Vector3>();
        List<Vector3> extendedOutlinePoints = new List<Vector3>();

        for (int i = 0; i < fillMesh.vertices.Length; i++)
        {
            Vector3 point = fillMesh.vertices[i];
            Vector3 prevPoint = fillMesh.vertices[(i - 1 + fillMesh.vertices.Length) % fillMesh.vertices.Length];
            Vector3 nextPoint = fillMesh.vertices[(i + 1) % fillMesh.vertices.Length];

            // Calcul de la direction moyenne pour lisser l'outline
            Vector3 direction = ((point - prevPoint).normalized + (nextPoint - point).normalized).normalized;
            Vector3 perpendicular = Vector3.Cross(direction, Vector3.forward).normalized;

            Vector3 outlinePoint = point;
            Vector3 extendedOutlinePoint = point + perpendicular * outlineThickness;

            outlinePoints.Add(outlinePoint);
            extendedOutlinePoints.Add(extendedOutlinePoint);
        }

        // Génération des triangles pour relier les points de l'outline
        List<int> outlineTriangles = new List<int>();
        int count = outlinePoints.Count;
        for (int i = 0; i < count; i++)
        {
            int nextIndex = (i + 1) % count;
            outlineTriangles.Add(i);
            outlineTriangles.Add(nextIndex);
            outlineTriangles.Add(i + count);

            outlineTriangles.Add(nextIndex);
            outlineTriangles.Add(nextIndex + count);
            outlineTriangles.Add(i + count);
        }

        outlineMesh.vertices = outlinePoints.Concat(extendedOutlinePoints).ToArray();
        outlineMesh.triangles = outlineTriangles.ToArray();
    }
    private int[] GetTriangles(List<Vector3> pointsPos)
    {
        int triangleAmount = pointsPos.Count - 2;
        List<int> newTriangles = new List<int>();

        for (int i = 0; i < triangleAmount; i++)
        {
            newTriangles.Add(0);
            newTriangles.Add(i + 2);
            newTriangles.Add(i + 1);
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

    public void SetToShrink()
    {
        bevelCount = shrinkBevelCount;
        heightFactor = shrinkHeightFactor;
    }
    public void SetToExtend()
    {
        bevelCount = extendBevelCount;
        heightFactor = extendHeightFactor;
    }
}