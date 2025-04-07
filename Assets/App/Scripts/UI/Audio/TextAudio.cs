using UnityEngine;
public class TextAudio : MonoBehaviour
{
    //[Header("Settings")]

    [Header("References")]
    [SerializeField] private SoundComponent textGameStartSC;
    [SerializeField] private SoundComponent textReadySC;
    [SerializeField] private SoundComponent textStartSC;
    [SerializeField] private SoundComponent textVictorySC;


    //[Space(10)]
    // RSO
    // RSF
    // RSP

    //[Header("Input")]
    //[Header("Output")]

    public void PlayVictorySound()
    {
        textVictorySC.PlayClip();
    }
    public void PlayStartSound()
    {
        textStartSC.PlayClip(true);
    }
    public void PlayReadySound()
    {
        textReadySC.PlayClip(true);
    }
    public void PlayGameStartSound()
    {
        textGameStartSC.PlayClip();
    }
}