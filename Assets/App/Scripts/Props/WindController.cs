using System.Collections;
using UnityEngine;

[System.Serializable]
public struct WindControl
{
    public float target;
    public float time;
}
public class WindController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] WindControl[] windControl;
    [SerializeField] bool isLooping;

    [Header("References")]
    [SerializeField] MyWindZone windZone;

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    //[Header("Input")]
    //[Header("Output")]
    private void Start()
    {
        StartCoroutine(ChangeWindForce());
    }
    IEnumerator ChangeWindForce()
    {
        if (isLooping)
        {
            for (int i = 0; i < windControl.Length; i++)
            {
                float elapsedTime = 0f;
                float duration = windControl[i].time;
                float startForce = windZone.GetWindForce(); // Assuming there's a method to get current wind force
                float targetForce = windControl[i].target;

                while (elapsedTime < duration)
                {
                    elapsedTime += Time.deltaTime;
                    float newForce = Mathf.Lerp(startForce, targetForce, elapsedTime / duration);
                    windZone.SetWindForce(newForce);
                    yield return null; // Wait for the next frame
                }

                windZone.SetWindForce(targetForce); // Ensure it reaches the exact target value
            }

            StartCoroutine(ChangeWindForce());
        }
        else
        {
            windZone.SetWindForce(windControl[0].target);
        }
    }
}