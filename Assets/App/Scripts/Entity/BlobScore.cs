using UnityEngine;
public class BlobScore : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] int score;
    //[Header("References")]

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    //[Header("Input")]
    //[Header("Output")]


    public void AddScore()
    {  
        score++;
    }
    public int GetScore()
    {
        return score;
    }
}