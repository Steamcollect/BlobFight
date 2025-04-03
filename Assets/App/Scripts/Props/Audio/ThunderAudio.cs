using UnityEngine;

public class ThunderAudio : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SoundComponent thunderLightningSC;
    [SerializeField] private SoundComponent thunderSparkleSC;
    [SerializeField] private ThunderProps thunderSpawner;
    public void PlayLightningSound()
    {
        thunderLightningSC.PlayClip();
    }
    public void PlaySparkleSound()
    {
        thunderSparkleSC.PlayClip();
    }
}