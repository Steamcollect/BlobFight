using UnityEngine;

public class SoundComponent : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Sound sound;

    [Header("Output")]
    [SerializeField] private RSE_PlayClipAt rsePlayClipAt;

    public void PlayClip()
    {
        rsePlayClipAt.Call(sound, transform.position);
    }
}