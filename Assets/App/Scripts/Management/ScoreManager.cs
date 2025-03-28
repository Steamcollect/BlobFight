using System.Linq;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] RSE_OnBlobDeath rseOnBlobDeath;
    [SerializeField] RSE_OnGameStart rseOnGameStart;
    [SerializeField] RSE_OnFightStart rseOnFightStart;
    [SerializeField] RSE_AddScore rseAddScore;

    [Header("Output")]
    [SerializeField] RSE_UpdateCrownVisual rseUpdateCrownVisual;
    [SerializeField] RSO_BlobInGame rsoBlobInGame;

    private SerializableDictionary<BlobMotor, int> scoreDictionary = new();

    private void OnEnable()
    {
        rseOnBlobDeath.action += UpdateCrownOwner;
        rseOnGameStart.action += SetUp;
        rseOnFightStart.action += GetScoreMax;
        rseAddScore.action += AddScore;
    }

    private void OnDisable()
    {
        rseOnBlobDeath.action -= UpdateCrownOwner;
        rseOnGameStart.action -= SetUp;
        rseOnFightStart.action -= GetScoreMax;
        rseAddScore.action -= AddScore;
    }

    private void SetUp()
    {
        scoreDictionary.Clear();
        rsoBlobInGame.Value.ForEach(blob => scoreDictionary[blob] = 0);
    }

    private void GetScoreMax()
    {
        if (!scoreDictionary.Any()) return;

        int maxScore = scoreDictionary.Values.Max();
        var topScorers = scoreDictionary.Where(p => p.Value == maxScore).Select(p => p.Key).ToList();

        rsoBlobInGame.Value.ForEach(blob => blob.DisableCrown());

        if (maxScore > 0)
        {
            topScorers.ForEach(blob => blob.EnableCrown());
            rseUpdateCrownVisual.Call(topScorers.Count == 1);
        }
    }

    private void UpdateCrownOwner(BlobMotor deadBlob)
    {
        deadBlob.DisableCrown();
        var aliveBlobs = scoreDictionary.Keys.Where(blob => blob.IsAlive()).ToList();

        if (!aliveBlobs.Any()) return;

        int highestScore = aliveBlobs.Max(blob => scoreDictionary[blob]);
        var bestPlayers = aliveBlobs.Where(blob => scoreDictionary[blob] == highestScore).ToList();

        if (bestPlayers.Any())
        {
            bestPlayers.ForEach(blob => blob.EnableCrown());
            rseUpdateCrownVisual.Call(bestPlayers.Count == 1);
        }
    }

    private void AddScore(BlobMotor blob)
    {
        if (scoreDictionary.ContainsKey(blob))
        {
            scoreDictionary[blob] += 1;
        }
    }
}