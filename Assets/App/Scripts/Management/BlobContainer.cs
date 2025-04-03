using UnityEngine;

public class BlobContainer : MonoBehaviour
{
    //[Header("Settings")]

    [Header("References")]
    [SerializeField] RSO_BlobInGame rsoBlobInGame;

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    [Header("Input")]
    [SerializeField] RSE_OnFightEnd rseOnFightEnd;
    [SerializeField] RSE_OnGameStart rseOnGameStart;

    //[Header("Output")]

    private void OnEnable()
    {
        rseOnFightEnd.action += SetBlobParent;
        rseOnGameStart.action += SetBlobParent;
    }

    private void OnDisable()
    {
        rseOnFightEnd.action -= SetBlobParent;
        rseOnGameStart.action -= SetBlobParent;
    }

    private void SetBlobParent()
    {
		for (int i = 0; i < rsoBlobInGame.Value.Count; i++)
		{
			rsoBlobInGame.Value[i].transform.SetParent(transform);
		}
	}

}