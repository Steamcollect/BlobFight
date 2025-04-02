using UnityEngine;
public class LevelMusic : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] Playlist[] playlists;

    //[Header("References")]

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    //[Header("Input")]
    [Header("Output")]
    [SerializeField] RSE_ChangeAmbianceMusic rseChangeAmbianceMusic;

    private void Start()
    {
        Invoke("LateStart", 0.1f);
    }
    private void LateStart()
    {
        rseChangeAmbianceMusic.Call(playlists);
    }
}