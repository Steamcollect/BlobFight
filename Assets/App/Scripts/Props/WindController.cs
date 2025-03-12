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
    [SerializeField] WindProps windZone;

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
                windZone.SetWindForce(windControl[i].target);
                yield return new WaitForSeconds(windControl[i].time);
            }
            StartCoroutine(ChangeWindForce());
        }
        else
        {
            windZone.SetWindForce(windControl[0].target);
        }
    }
}