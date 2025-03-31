using UnityEngine;
public class BlobStaminaVisual : MonoBehaviour
{
    //[Header("Settings")]

    [Header("References")]
    [SerializeField] BlobStamina blobStamina;

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    //[Header("Input")]
    //[Header("Output")]

    private void Update()
    {
        float value = blobStamina.GetStaminaPercentage();
        transform.localScale = new Vector3(value, value, value);
    }
}