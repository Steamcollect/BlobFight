using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DebugUI : MonoBehaviour
{
    public TextMeshProUGUI[] blobHealthTexts;

    BlobHealth[] blobHealths;


    private void Update()
    {
        blobHealths = FindObjectsByType<BlobHealth>(FindObjectsSortMode.None);

        Debug.Log("Found " + blobHealths.Length + " blob healths");

        for (int i = 0; i < blobHealthTexts.Length; i++)
        {
            blobHealthTexts[i].text = "";
        }

        for (int i = 0; i < blobHealths.Length; i++)
        {
            int value = Mathf.RoundToInt((blobHealths[i].pushBackPercentage * 100f));
            blobHealthTexts[i].text = blobHealths[i].transform.parent.parent.name + ": " + value.ToString();
        }

    }
}