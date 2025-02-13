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
    
    private Vector3 velocity = Vector3.zero;
    private float zoomVelocity = 0f;

    [Space(10)]
    [SerializeField] float shakeSpeed;
    float shakeOffset;

    [Header("References")]
    [SerializeField] Camera cam;

    Coroutine shakeCoroutine;

    [Space(10)]
    [SerializeField] RSO_BlobInGame rsoBlobInGame;

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
        if (rsoBlobInGame.Value.Count == 0) return;

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
        foreach (BlobMotor blob in rsoBlobInGame.Value)
        {
            sum += blob.joint.GetJointsCenter();
        }
        return sum / rsoBlobInGame.Value.Count;
    }

    private float CalculateMaxDistance(Vector2 center)
    {
        float maxDistance = 0f;
        foreach (BlobMotor blob in rsoBlobInGame.Value)
        {
            float distance = Vector2.Distance(center, blob.joint.GetJointsCenter());
            if (distance > maxDistance)
            {
                maxDistance = distance;
            }
        }
        return maxDistance * zoomMultiplier;
    }

    void Shake(float range, float time)
    {
        StartCoroutine(CameraShake(range, time));
    }
    IEnumerator CameraShake(float range, float time)
    {
        if (range > Mathf.Abs(shakeOffset))
        {
            float elapsedTime = 0f;
            while (elapsedTime < time)
            {
                float dampingFactor = 1 - (elapsedTime / time);
                shakeOffset = range * dampingFactor;
                elapsedTime += Time.deltaTime;

                //transform.rotation = Quaternion.Euler(0, 0, shakeOffset);
                yield return null;
            }

            shakeOffset = 0f;
            transform.rotation = Quaternion.Euler(0, 0, shakeOffset);
        }
    }
}