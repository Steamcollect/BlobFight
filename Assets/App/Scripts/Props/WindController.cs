using System.Collections;
using UnityEngine;

public class WindController : MonoBehaviour
{
    [System.Serializable]
    private struct WindControl
    {
        public float target;
        public float time;
    }

    [Header("Settings")]
    [SerializeField] private bool isLooping;
    [SerializeField] private WindControl[] windControl;

    [Header("References")]
    [SerializeField] MyWindZone windZone;

    private void Start()
    {
        StartCoroutine(ChangeWindForce());
    }

    private IEnumerator ChangeWindForce()
    {
        foreach (var control in windControl)
        {
            float elapsedTime = 0f;
            float duration = control.time;
            float startForce = windZone.GetWindForce(); // Assuming there's a method to get current wind force
            float targetForce = control.target;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float newForce = Mathf.Lerp(startForce, targetForce, elapsedTime / duration);
                windZone.SetWindForce(newForce);
                yield return null; // Wait for the next frame
            }

            windZone.SetWindForce(targetForce); // Ensure it reaches the exact target value
        }

        if (isLooping)
        {
            StartCoroutine(ChangeWindForce());
        }
    }
}