using UnityEngine;
public class VialAudio : MonoBehaviour
{
    //[Header("Settings")]

    [Header("References")]
    [SerializeField] private SoundComponent vialHitSC;
    [SerializeField] private SoundComponent vialDestroySC;

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    //[Header("Input")]
    //[Header("Output")]
    public void PlayVialHitSound()
    {
        vialHitSC.PlayClip();
    }
    public void PlayVialDestroySound()
    {
        vialDestroySC.PlayClip();
    }
}