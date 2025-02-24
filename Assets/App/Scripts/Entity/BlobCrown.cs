using UnityEngine;

public class BlobCrown : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] Vector2 shrinkPos;
    [SerializeField] Vector2 extendPos;
    [SerializeField] float smoothTime;

    [Space(5)]
    [SerializeField] float rotationAmount;
    [SerializeField] float rotationTime;
    [SerializeField] float velocityRotDiviser;
    Vector2 velocity;
    Vector2 posOffset;
    private float rotationDelta;
    private float rotationVelocity;

    [Header("References")]
    [SerializeField] BlobMotor motor;
    [SerializeField] BlobJoint joint;

    [Space(5)]
    [SerializeField] GameObject crown;
    
    

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    //[Header("Input")]
    //[Header("Output")]

    private void OnEnable()
    {
        motor.enableCrown += EnableCrown;
        motor.disableCrown += DisableCrown;
    }
    private void OnDisable()
    {
        motor.enableCrown -= EnableCrown;
        motor.disableCrown -= DisableCrown;
    }

    private void Update()
    {
        UpdateVisual();
    }

    void EnableCrown()
    {
        crown.SetActive(true);
    }

    void DisableCrown()
    {
        crown.SetActive(false);
    }
    void SetPosCrownShrink()
    {
        posOffset = shrinkPos;
    }
    void SetPosCrownExtend()
    {
        posOffset = extendPos;
    }
    void UpdateVisual()
    {
        crown.transform.position = Vector2.SmoothDamp(crown.transform.position, joint.GetJointsCenter() + posOffset, ref velocity, smoothTime);

        float targetRot = -(velocity / velocityRotDiviser * rotationAmount).x;
        rotationDelta = Mathf.SmoothDamp(rotationDelta, targetRot, ref rotationVelocity, rotationTime);
        crown.transform.eulerAngles = new Vector3(0, 0, Mathf.Clamp(rotationDelta, -60, 60));
    }
}