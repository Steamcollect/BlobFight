using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float minSize = 5f;
    [SerializeField] float maxSize = 15f;
    [SerializeField] float zoomMultiplier;

    [Space(10)]
    [SerializeField] float moveSmoothTime = 0.2f;
    [SerializeField] float zoomSmoothTime = 0.2f;

    [Header("References")]
    [SerializeField] List<BlobJoint> blobs;
    [SerializeField] Camera cam;

    private Vector3 velocity = Vector3.zero;
    private float zoomVelocity = 0f;

    private void LateUpdate()
    {
        if (blobs.Count == 0) return;

        Vector2 center = CalculateCenter();
        float distance = CalculateMaxDistance(center);

        // SmoothDamp for position
        Vector3 targetPosition = new Vector3(center.x, center.y, cam.transform.position.z);
        cam.transform.position = Vector3.SmoothDamp(cam.transform.position, targetPosition, ref velocity, moveSmoothTime);

        // SmoothDamp for zoom
        float targetSize = Mathf.Clamp(distance, minSize, maxSize);
        cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, targetSize, ref zoomVelocity, zoomSmoothTime);
    }

    private Vector2 CalculateCenter()
    {
        Vector2 sum = Vector2.zero;
        foreach (BlobJoint blob in blobs)
        {
            sum += blob.GetJointsCenter();
        }
        return sum / blobs.Count;
    }

    private float CalculateMaxDistance(Vector2 center)
    {
        float maxDistance = 0f;
        foreach (BlobJoint blob in blobs)
        {
            float distance = Vector2.Distance(center, blob.GetJointsCenter());
            if (distance > maxDistance)
            {
                maxDistance = distance;
            }
        }
        return maxDistance * zoomMultiplier;
    }
}
