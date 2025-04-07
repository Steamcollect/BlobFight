using System.Collections.Generic;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private RSE_OnBlobDeath rseOnBlobDeath;
    [SerializeField] private RSE_OnFightStart rseOnFightStart;
    [SerializeField] private RSE_Message rseMessage;

    [Header("Output")]
    [SerializeField] private RSO_BlobInGame rsoBlobInGame;
    [SerializeField] private RSE_OnFightEnd rseOnFightEnd;
    [SerializeField] private RSE_AddScore rseAddScore;
    [SerializeField] private SSO_ListFightText listFightText;

    private List<BlobMotor> blobs = new();
    private int randomText = 0;
    private void OnEnable()
    {
        rseOnBlobDeath.action += CheckBlobCount;
        rseOnFightStart.action += Setup;
    }

    private void OnDisable()
    {
        rseOnBlobDeath.action -= CheckBlobCount;
        rseOnFightStart.action -= Setup;
    }

    private void Setup()
    {
        blobs = new(rsoBlobInGame.Value);
    }

    private void CheckBlobCount(BlobMotor blob)
    {
        if (blobs != null && blobs.Contains(blob))
        {
            var tempBlob = blob;
            blobs.Remove(tempBlob);

            randomText = Random.Range(0, listFightText.victoryText.Count);
            if (blobs.Count == 0)
            {
                rseOnFightEnd.Call();
                rseMessage.Call($"{listFightText.victoryText[randomText]}", 1f, listFightText.colorMessage, MessageManager.MessageTexteType.Win);
            }
            else if (blobs.Count == 1)
            {
                rseAddScore.Call(blobs[0]);
                rseOnFightEnd.Call();
                rseMessage.Call($"{listFightText.victoryText[randomText]}", 1f, blobs[0].GetColor().fillColor, MessageManager.MessageTexteType.Win);
            }
            
        }
    }
}