using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float minSize;
    [SerializeField] private float maxSize;
    [SerializeField] private float zoomMultiplier;
    [SerializeField] private bool cameraLock;
    [SerializeField] private float moveSmoothTime;
    [SerializeField] private float zoomSmoothTime;
    [SerializeField] private Vector3 centerOffset;
    [SerializeField] private Vector2 minBounds; // Limite minimale (x, y)
    [SerializeField] private Vector2 maxBounds; // Limite maximale (x, y)
    [SerializeField] private float shakeSpeed;

    [Header("References")]
    [SerializeField] private Camera cam;

    [Header("Input")]
    [SerializeField] private RSE_CameraShake rseCameraShake;
    [SerializeField] private RSE_OnFightEnd rseOnFightEnd;
    [SerializeField] private RSE_OnFightStart rseOnFightStart;

    [Header("Output")]
    [SerializeField] private RSO_BlobInGame rsoBlobInGame;
    [SerializeField] private RSO_SettingsSaved rsoSettingsSaved;

    private bool lockCam = true;
    private Vector3 velocity = Vector3.zero;
    private float zoomVelocity = 0f;
    private float shakeOffset = 0;
    private Vector3 posOffset = Vector3.zero;

    private void OnEnable()
    {
        rseCameraShake.action += Shake;
        rseOnFightStart.action += UnlockCamera;
        rseOnFightEnd.action += LockCamera;
    }

    private void OnDisable()
    {
        rseCameraShake.action -= Shake;
        rseOnFightStart.action -= UnlockCamera;
        rseOnFightEnd.action -= LockCamera;
    }

    private void LockCamera()
    {
        lockCam = true;
    }

    private void UnlockCamera()
    {
        lockCam = false;
    }

    private void LateUpdate()
    {
        if (lockCam || cameraLock || rsoBlobInGame.Value.Count == 0) return;

        Vector2 center = CalculateCenter();
        float distance = CalculateMaxDistance(center);

        // SmoothDamp for position
        Vector3 targetPosition = new Vector3(center.x, center.y, cam.transform.position.z) + centerOffset;
        cam.transform.position = Vector3.SmoothDamp(cam.transform.position, targetPosition, ref velocity, moveSmoothTime) + posOffset;

        // SmoothDamp for zoom
        cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, Mathf.Clamp(distance, minSize, maxSize), ref zoomVelocity, zoomSmoothTime);

        // Apply bounds clamping
        float halfWidth = cam.orthographicSize * cam.aspect;
        float halfHeight = cam.orthographicSize;

        float clampedX = Mathf.Clamp(cam.transform.position.x, minBounds.x + halfWidth, maxBounds.x - halfWidth);
        float clampedY = Mathf.Clamp(cam.transform.position.y, minBounds.y + halfHeight, maxBounds.y - halfHeight);

        cam.transform.position = new Vector3(clampedX, clampedY, cam.transform.position.z);
    }

    private Vector2 CalculateCenter()
    {
        Vector2 sum = Vector2.zero;
        int aliveCount = 0;

        // Calculate center position of all alive blobs
        foreach (BlobMotor blob in rsoBlobInGame.Value)
        {
            if (blob.IsAlive())
            {
                sum += blob.GetPhysics().GetCenter();
                aliveCount++;
            }
        }

        return aliveCount > 0 ? sum / aliveCount : Vector2.zero;
    }

    private float CalculateMaxDistance(Vector2 center)
    {
        float maxDistance = 0f;

        foreach (BlobMotor blob in rsoBlobInGame.Value)
        {
            if (blob.IsAlive())
            {
                maxDistance = Mathf.Max(maxDistance, Vector2.Distance(center, blob.GetPhysics().GetCenter()));
            }
        }

        return maxDistance * zoomMultiplier;
    }

    private void Shake(float range, float time)
    {
        if (rsoSettingsSaved.Value.screenShake)
        {
            StartCoroutine(CameraShake(range, time));
        }
    }

    private IEnumerator CameraShake(float magnitude, float time)
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        Vector3[] corners = new Vector3[4]
        {
            new Vector3(minBounds.x, minBounds.y, 0),
            new Vector3(maxBounds.x, minBounds.y, 0),
            new Vector3(maxBounds.x, maxBounds.y, 0),
            new Vector3(minBounds.x, maxBounds.y, 0)
        };

        // Draw rectangle bounds
        for (int i = 0; i < 4; i++)
        {
            Gizmos.DrawLine(corners[i], corners[(i + 1) % 4]);
        }
    }
}