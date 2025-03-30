using UnityEngine;

public class BlobCrown : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Vector2 shrinkPos;
    [SerializeField] private Vector2 extendPos;
    [SerializeField] private float smoothTime;
    [SerializeField] private float rotationAmount;
    [SerializeField] private float rotationTime;
    [SerializeField] private float velocityRotDiviser;

    [Header("References")]
    [SerializeField] private BlobMotor motor;
    [SerializeField] private BlobPhysics joint;
    [SerializeField] private BlobMovement movement;
    [SerializeField] private GameObject crownsContent;
    [SerializeField] private GameObject goldCrown;
    [SerializeField] private GameObject silverCrown;

    [Header("Input")]
    [SerializeField] private RSE_UpdateCrownVisual rseUpdateCrownVisual;

    private Vector2 velocity = Vector2.zero;
    private Vector2 posOffset = Vector2.zero;
    private float rotationDelta = 0;
    private float rotationVelocity = 0;

    private void OnEnable()
    {
        motor.enableCrown += EnableCrown;
        motor.disableCrown += DisableCrown;

        movement.onExtend += OnExtend;
        movement.onShrink += OnShrink;

        rseUpdateCrownVisual.action += UpdateVisual;
    }

    private void OnDisable()
    {
        motor.enableCrown -= EnableCrown;
        motor.disableCrown -= DisableCrown;

        movement.onExtend -= OnExtend;
        movement.onShrink -= OnShrink;

        rseUpdateCrownVisual.action -= UpdateVisual;
    }

    private void Update()
    {
        Move();
    }

    private void EnableCrown()
    {
        crownsContent.SetActive(true);
    }

    private void DisableCrown()
    {
        crownsContent.SetActive(false);
    }

    private void UpdateVisual(bool isGold)
    {
        goldCrown.SetActive(isGold);
        silverCrown.SetActive(!isGold);
    }

    private void OnShrink()
    {
        posOffset = shrinkPos;
    }

    private void OnExtend()
    {
        posOffset = extendPos;
    }

    private void Move()
    {
        if(crownsContent.activeInHierarchy)
        {
            crownsContent.transform.position = Vector2.SmoothDamp(crownsContent.transform.position, joint.GetCenter() + posOffset, ref velocity, smoothTime);

            float targetRot = (velocity / velocityRotDiviser * rotationAmount).x;
            rotationDelta = Mathf.SmoothDamp(rotationDelta, targetRot, ref rotationVelocity, rotationTime);
            crownsContent.transform.eulerAngles = new Vector3(0, 0, Mathf.Clamp(rotationDelta, -60, 60));
        }
    }
}