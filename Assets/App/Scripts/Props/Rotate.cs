using UnityEngine;
public class Rotate : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float rotationSpeed;

    [Header("Input")]
    [SerializeField] private RSE_OnPause rseOnPause;
    [SerializeField] private RSE_OnResume rseOnResume;

    private bool isPaused = false;

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

    private void Update()
    {
        if (!isPaused && gameObject.activeInHierarchy)
        {
            Rotation();
        }
    }

    private void Pause()
    {
        isPaused = true;
    }

    private void Resume()
    {
        isPaused = false;
    }

    private void Rotation()
    {
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }
}