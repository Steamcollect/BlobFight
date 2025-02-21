using UnityEngine;
public class RidgidbodyMotor : MonoBehaviour
{
    [Header("Settings")]
    Vector3 velocity;
    float angularVelocity;

    RigidbodyType2D bodyType;

    [Header("References")]
    [SerializeField] Rigidbody2D rb;

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    //[Header("Input")]
    //[Header("Output")]

    void FreezRigidbody()
    {
        velocity = rb.velocity;
        angularVelocity = rb.angularVelocity;

        rb.bodyType = RigidbodyType2D.Static;
    }
    void UnfreezVelocity()
    {
        rb.bodyType = bodyType;

        rb.velocity = velocity;
        rb.angularVelocity = angularVelocity;
    }

    private void OnValidate()
    {
        bodyType = rb.bodyType;
    }
}