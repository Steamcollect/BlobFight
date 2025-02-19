using UnityEngine;

[System.Serializable]
public struct BlobStatistics
{
    public float moveSpeed;
    public float gravity;

    [Space(5)]
    public float drag;

    [Space(5)]
    public float mass;
    
    [Space(5)]
    public float distanceMult;
    [Range(0, 1)] public float damping;
    public float frequency;
}