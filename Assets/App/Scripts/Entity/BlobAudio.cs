using UnityEngine;
public class BlobAudio : MonoBehaviour
{
    //[Header("Settings")]

    [Header("References")]
    [SerializeField] SoundComponent dashSoundComponent;

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    //[Header("Input")]
    //[Header("Output")]
    public void PlayDashClip()
    {
        dashSoundComponent.PlayClip();
    }
}