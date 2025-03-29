using UnityEngine;

public class ThunderAudio : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SoundComponent thunderSoundComponent;
    [SerializeField] private ThunderProps thunderSpawner;

    private void OnEnable()
    {
        thunderSpawner.onThunderSpawn += PlayThunderClip;
    }

    private void OnDisable()
    {
        thunderSpawner.onThunderSpawn -= PlayThunderClip;
    }

    private void PlayThunderClip()
    {
        thunderSoundComponent.PlayClip();
    }
}