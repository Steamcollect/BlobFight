using UnityEngine;

public class BlobCrown : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] Vector2 shrinkPos;
    [SerializeField] Vector2 extendPos;
    [SerializeField] float smoothTime;

    Vector3 velocity;
    Vector2 posOffset;

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

    public void EnableCrown()
    {
        crown.SetActive(true);
    }

    public void DisableCrown()
    {
        crown.SetActive(false);
    }

    void UpdateVisual()
    {
        crown.transform.position = Vector3.SmoothDamp(crown.transform.position, joint.GetJointsCenter() + posOffset, ref velocity, smoothTime);
    }
}