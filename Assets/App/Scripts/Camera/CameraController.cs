using System.Collections;
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

    [Space(10)]
    [SerializeField] Vector3 centerOffset;

    private Vector3 velocity = Vector3.zero;
    private float zoomVelocity = 0f;

    private Vector3 posOffset;

    [Header("Bounds")]
    [SerializeField] Vector2 minBounds; // Limite minimale (x, y)
    [SerializeField] Vector2 maxBounds; // Limite maximale (x, y)

    [Space(10)]
    [SerializeField] float shakeSpeed;
    float shakeOffset;

    [Header("References")]
    [SerializeField] Camera cam;

    [Space(10)]
    [SerializeField] RSO_BlobInGame rsoBlobInGame;
    [SerializeField] RSO_SettingsSaved rsoSettingsSaved;

    [Header("Input")]
    [SerializeField] RSE_CameraShake rseCameraShake;

    private void OnEnable()
    {
        rseCameraShake.action += Shake;
    }
    private void OnDisable()
    {
        rseCameraShake.action -= Shake;
    }
    private void LateUpdate()
    {
        if (rsoBlobInGame.Value.Count <= 1) return;

        Vector2 center = CalculateCenter();
        float distance = CalculateMaxDistance(center);

        // SmoothDamp for position
        Vector3 targetPosition = new Vector3(center.x, center.y, cam.transform.position.z) + centerOffset;
        cam.transform.position = Vector3.SmoothDamp(cam.transform.position, targetPosition, ref velocity, moveSmoothTime) + posOffset;

        // SmoothDamp for zoom
        float targetSize = Mathf.Clamp(distance, minSize, maxSize);
        cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, targetSize, ref zoomVelocity, zoomSmoothTime);

        //Bounds fixes
        float halfWidth = cam.orthographicSize * cam.aspect;
        float halfHeight = cam.orthographicSize;

        float clampedX = Mathf.Clamp(cam.transform.position.x, minBounds.x + halfWidth, maxBounds.x - halfWidth);
        float clampedY = Mathf.Clamp(cam.transform.position.y, minBounds.y + halfHeight, maxBounds.y - halfHeight);

        cam.transform.position = new Vector3(clampedX, clampedY, cam.transform.position.z);
    }

    private Vector2 CalculateCenter()
    {
        Vector2 sum = Vector2.zero;
        foreach (BlobMotor blob in rsoBlobInGame.Value)
        {
            if (!blob.IsAlive()) continue;

            sum += blob.GetPhysics().GetCenter();
        }
        return sum / rsoBlobInGame.Value.Count;
    }

    private float CalculateMaxDistance(Vector2 center)
    {
        float maxDistance = 0f;
        foreach (BlobMotor blob in rsoBlobInGame.Value)
        {
            if (!blob.IsAlive()) continue;

            float distance = Vector2.Distance(center, blob.GetPhysics().GetCenter());
            if (distance > maxDistance)
            {
                maxDistance = distance;
            }
        }
        return maxDistance * zoomMultiplier;
    }

    void Shake(float range, float time)
    {
        if (rsoSettingsSaved.Value.screenShake)
        {
            StartCoroutine(CameraShake(range, time));
        }
    }
    IEnumerator CameraShake(float magnitude, float time)
    {
        if (magnitude > Mathf.Abs(shakeOffset))
        {
            float elapsed = 0f;

            while (elapsed < time)
            {
                float currentMagnitude = Mathf.Lerp(magnitude, 0, elapsed / time);

                float x = Random.Range(-1f, 1f) * currentMagnitude;
                float y = Random.Range(-1f, 1f) * currentMagnitude * 0.5f;

                posOffset =new Vector3(x, y, 0);
                elapsed += Time.deltaTime;

                yield return null;
            }
            posOffset = Vector3.zero;
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Vector3 bottomLeft = new Vector3(minBounds.x, minBounds.y, 0);
        Vector3 bottomRight = new Vector3(maxBounds.x, minBounds.y, 0);
        Vector3 topLeft = new Vector3(minBounds.x, maxBounds.y, 0);
        Vector3 topRight = new Vector3(maxBounds.x, maxBounds.y, 0);

        Gizmos.DrawLine(bottomLeft, bottomRight); // Bas
        Gizmos.DrawLine(bottomRight, topRight);   // Droite
        Gizmos.DrawLine(topRight, topLeft);       // Haut
        Gizmos.DrawLine(topLeft, bottomLeft);     // Gauche
    }
}