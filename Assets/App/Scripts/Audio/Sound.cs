using UnityEngine;

[System.Serializable]
public struct Sound
{
    public float volumeMultiplier;

    [Range(0, 1), Tooltip("0 -> 2D || 1 -> 3D")] public float spatialBlend;

    public AudioClip[] clips;
}