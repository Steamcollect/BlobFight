using UnityEngine;
public class LightFlash : MonoBehaviour
{
    //[Header("Settings")]

    [Header("References")]
    [SerializeField] Animator anim;

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    [Header("Input")]
    [SerializeField] RSE_LightFlash rseLightFlash;

    //[Header("Output")]

    private void OnEnable()
    {
        rseLightFlash.action += Play;
    }
    private void OnDisable()
    {
        rseLightFlash.action -= Play;
    }

    void Play()
    {
        anim.SetTrigger("Flash");
    }
}