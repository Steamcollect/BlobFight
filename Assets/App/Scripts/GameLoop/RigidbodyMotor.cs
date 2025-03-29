using UnityEngine;

public class RigidbodyMotor : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;

    [Header("Input")]
    [SerializeField] private RSE_OnPause rseOnPause;
    [SerializeField] private RSE_OnResume rseOnResume;

    private Vector3 velocity;
    private float angularVelocity;
    private RigidbodyType2D bodyType;

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

    private void Pause()
    {
        velocity = rb.velocity;
        angularVelocity = rb.angularVelocity;
        bodyType = rb.bodyType;

        rb.bodyType = RigidbodyType2D.Static;
    }

    private void Resume()
    {
        rb.bodyType = bodyType;

        if (rb.bodyType != RigidbodyType2D.Static)
        {
            rb.velocity = velocity;
            rb.angularVelocity = angularVelocity;
        }
    }

    private void GetRigidbody()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetScripts(RSE_OnPause onPause, RSE_OnResume onResume)
    {
        GetRigidbody();

        rseOnPause = onPause;
        rseOnResume = onResume;
    }
}