using UnityEngine;

[CreateAssetMenu(fileName = "SSO_BlobVisuals", menuName = "SSO/_/SSO_BlobVisuals")]
public class SSO_BlobVisuals : ScriptableObject
{
    public BlobInitializeStatistic[] blobs;

    private void OnValidate()
    {
        for (int i = 0; i < blobs.Length; i++)
        {
            if (blobs[i].color.autoSetOutlineColor)
            {
                blobs[i].color.outlineColor = new Color(
                    blobs[i].color.fillColor.r - .2f,
                    blobs[i].color.fillColor.g - .2f,
                    blobs[i].color.fillColor.b - .2f,
                    1
                    );
            }
        }
    }
}

[System.Serializable]
public struct BlobInitializeStatistic
{
    public string blobName;
    public BlobColor color;

    [Space(10)]
    public LayerMask layer;
}

[System.Serializable]
public struct BlobColor
{
    public Color fillColor, outlineColor;
    public bool autoSetOutlineColor;
}