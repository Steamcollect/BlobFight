using UnityEngine;

public class ThunderAudio : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SoundComponent thunderSoundComponent;
    [SerializeField] private ThunderProps thunderSpawner;

    private void OnEnable()
    {
        thunderSpawner.onSoundPlay += PlayThunderClip;
    }

    private void OnDisable()
    {
        thunderSpawner.onSoundPlay -= PlayThunderClip;
    }

    private void PlayThunderClip()
    {
        thunderSoundComponent.PlayClip();
    }
}