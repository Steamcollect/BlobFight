using UnityEngine;

public class BlobStaminaVisual : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Color colorDisable;
    [SerializeField] private Color colorEnable;

    [Header("References")]
    [SerializeField] BlobStamina blobStamina;
    [SerializeField] BlobMovement blobMovement;

    private void Update()
    {
        float value = blobStamina.GetStaminaPercentage();
        transform.localScale = new Vector3(value, value, value);

        if (value <= blobMovement.GetStaminaPercentageRequire() && !blobMovement.IsExtend())
        {
            GetComponent<SpriteRenderer>().color = colorDisable;
        }
        else
        {
            GetComponent<SpriteRenderer>().color = colorEnable;
        }
    }
}