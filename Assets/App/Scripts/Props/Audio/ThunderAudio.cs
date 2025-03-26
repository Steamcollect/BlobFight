using UnityEngine;
public class ThunderAudio : MonoBehaviour
{
    //[Header("Settings")]

    [Header("References")]
    [SerializeField] private SoundComponent thunderSoundComponent;
    [Space(20)]
    [SerializeField] private ThunderProps thunderSpawner;

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    //[Header("Input")]
    //[Header("Output")]
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