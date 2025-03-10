using System.Collections;
using UnityEngine;
public class ThunderProps : MonoBehaviour
{
    //[Header("Settings")]

    [Header("References")]
    [SerializeField] Collider2D thunderCollider;

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    //[Header("Input")]
    //[Header("Output")]

    public void EnableCollider()
    {
        thunderCollider.enabled = true;
    }
    public void DisableCollider()
    {
        thunderCollider.enabled = false;
    }
    public void OnEndAnimation()
    {
        gameObject.SetActive(false);
    }
}