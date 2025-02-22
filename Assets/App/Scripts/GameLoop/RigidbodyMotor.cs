using UnityEngine;
public class RigidbodyMotor : MonoBehaviour, IPausable
{
    [Header("Settings")]
    Vector3 velocity;
    float angularVelocity;

    RigidbodyType2D bodyType;

    [Header("References")]
    [SerializeField, ContextMenuItem("Get Rigidbody", "GetRigidbody")] Rigidbody2D rb;

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    [Header("Input")]
    [SerializeField] RSE_OnPause rseOnPause;
    [SerializeField] RSE_OnResume rseOnResume;

    //[Header("Output")]

    private void OnEnable()
    {
        rseOnPause.action += Pause;
        rseOnResume.action += Resume;
    }
    private void OnDisable()
    {
        rseOnPause.action -= Pause;
        rseOnResume.action -= Resume;
    }

    public void Pause()
    {
        velocity = rb.velocity;
        angularVelocity = rb.angularVelocity;
        bodyType = rb.bodyType;

        rb.bodyType = RigidbodyType2D.Static;
    }
    public void Resume()
    {
        rb.bodyType = bodyType;
        rb.velocity = velocity;
        rb.angularVelocity = angularVelocity;
    }

    void GetRigidbody()
    {
        rb = GetComponent<Rigidbody2D>();
    }
}