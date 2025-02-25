using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class ScoreManager : MonoBehaviour
{
    //[Header("Settings")]

    [Header("References")]

    [Space(10)]
    // RSO
    [SerializeField] RSO_BlobInGame rsoBlobInGame;
    // RSF
    // RSP

    //[Header("Input")]
    //[Header("Output")]
    private List<int> score = new List<int>();
    private void Start()
    {
        GetScore();
        GetScoreMax();
    }
    private void GetScore()
    {
        BlobMotor blob = new BlobMotor();
        
        for (int i = 0; rsoBlobInGame.Value.Count > i; i++)
        {
            blob = rsoBlobInGame.Value[i];
            score.Add(blob.GetScore());

        }
        
    }
    private void GetScoreMax()
    {
        BlobMotor blob = new BlobMotor();
        int max = score.Max();
        for (int i = 0; rsoBlobInGame.Value.Count > i; i++)
        {
            blob = rsoBlobInGame.Value[i];
            if (max == blob.GetScore())
            {
                if(max != 0)
                {
                    blob.EnableCrown();
                }
                else
                {
                    blob.DisableCrown();
                }
                
            }
            else
            {
                blob.DisableCrown();
            }
        }
    }
}